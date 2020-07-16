using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Nito.AsyncEx;
using YaWhois;
using MongoDB.Driver;
using VwM.BackgroundServices;
using VwM.Extensions;
using VwM.BackgroundServices.Whois;
using VwM.Database.Collections;
using VwM.Database.Server;


namespace VwM.Hubs
{
    public class WhoisHub : GenericHub<WhoisHub, WhoisRequestQueue, WhoisTaskQueue>
    {
        private readonly WhoisCollection _whois;
        private readonly DatabaseStatus _dbStatus;


        public WhoisHub(
            ILogger<WhoisHub> logger,
            IStringLocalizer<WhoisHub> localizer,
            WhoisRequestQueue queue,
            IBackgroundTaskQueue<WhoisTaskQueue> taskQueue,
            WhoisCollection whois,
            DatabaseStatus serverStatus) : base(logger, localizer, queue, taskQueue)
        {
            _whois = whois;
            _dbStatus = serverStatus;
        }


        public async Task ClientReady(string id, Guid queueId, string language)
        {
            var login = Context.User.GetLogin();
            var client = Clients.Client(id);

            if (queueId == Guid.Empty)
            {
                _logger.LogWarning($"Client {login} | {id} has passed empty queueId.");
                return;
            }

            if (!_queue.Queue.TryRemove(queueId, out IEnumerable<WhoisDto> dtos))
            {
                _logger.LogError($"Failed remove Ping Request {queueId} for client {id}.");
                await client.SendAsync("Exception", "Queue error.");
                return;
            }

            _logger.LogInformation($"User {login} has started Whois Request {queueId} with {dtos.Count()} host(s).");

            var culture = await GetCultureAsync(id, language);

            _tasks.QueueTask(async token =>
            {
                if (token.IsCancellationRequested)
                    return;

                Thread.CurrentThread.CurrentCulture =
                Thread.CurrentThread.CurrentUICulture = culture;

                var taskFactory = new TaskFactory(TaskScheduler.Current);
                var cde = new AsyncCountdownEvent(dtos.Count());
                var tasks = new List<Task>();

                var whois = new YaWhoisClient();
                whois.ResponseParsed += Whois_ResponseParsed;
                whois.ExceptionThrown += Whois_ExceptionThrown;

                foreach (var host in dtos.Select(a => a.Hostname))
                {
                    var data = new YaWhoisData()
                    {
                        Client = client,
                        ClientId = id,
                        Object = host
                    };

                    tasks.Add(taskFactory.StartNew((d) =>
                    {
                        whois.QueryAsync(host, token: token, value: d).Wait();
                        cde.Signal();
                    }, data, token));
                }

                try
                {
                    Task.WaitAll(tasks.ToArray());
                }
                catch (Exception e)
                {
                    var obj = e.Data["object"].ToString();
                    _logger.LogError(e, $"Whois {obj} failed (clientId: {id}).");
                }

                await cde.WaitAsync();
            });
        }


        private void Whois_ExceptionThrown(object sender, YaWhoisClientEventArgs e)
        {
            var data = (YaWhoisData)e.Value;
            e.Exception.Data.Add("object", data.Object);
            data.Client.SendAsync("Result", data.Object, e.Exception.Message).Wait();
        }


        private void Whois_ResponseParsed(object sender, YaWhoisClientEventArgs e)
        {
            var data = (YaWhoisData)e.Value;

            if (string.IsNullOrEmpty(e.Response))
                data.Client.SendAsync("Result", data.Object, "").Wait();

            if (_dbStatus.Connected)
                UpsertRecordAsync(data.Object, e.Response).Wait();

            data.Client.SendAsync("Result", data.Object, e.Response).Wait();
        }


        private async Task UpsertRecordAsync(string host, string response)
        {
            var doc = await _whois.GetFirstOrDefaultAsync(x => x.Hostname == host);

            if (doc != null)
            {
                var update = Builders<Database.Models.Whois>.Update
                    .Set(o => o.Updated, DateTime.UtcNow)
                    .Set(o => o.Result, response);

                await _whois.UpdateOneAsync(x => x.Id == doc.Id, update);
            }
            else
            {
                var now = DateTime.UtcNow;
                doc = new Database.Models.Whois
                {
                    Created = now,
                    Updated = now,
                    Hostname = host,
                    Result = response
                };

                await _whois.InsertOneAsync(doc);
            }
        }


        private class YaWhoisData
        {
            public IClientProxy Client;
            public string ClientId;
            public string Object;
        }
    }
}
