using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Nito.AsyncEx;
using Whois;
using Whois.Models;
using Newtonsoft.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using VwM.BackgroundServices;
using VwM.Extensions;
using VwM.BackgroundServices.Whois;
using VwM.Database.Collections;
using MongoDB.Driver;

namespace VwM.Hubs
{
    public class WhoisHub : GenericHub<WhoisHub, WhoisRequestQueue, WhoisTaskQueue>
    {
        private readonly WhoisCollection _whois;


        public WhoisHub(
            ILogger<WhoisHub> logger,
            IStringLocalizer<WhoisHub> localizer,
            WhoisRequestQueue queue,
            IBackgroundTaskQueue<WhoisTaskQueue> taskQueue,
            WhoisCollection whois) : base(logger, localizer, queue, taskQueue)
        {
            _whois = whois;
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

                var whois = new WhoisLookup();
                var taskFactory = new TaskFactory(TaskScheduler.Current);
                var cde = new AsyncCountdownEvent(dtos.Count());

                foreach (var host in dtos.Select(a => a.Hostname))
                {
                    await taskFactory.StartNew(async () =>
                    {
                        try
                        {
                            var response = await whois.LookupAsync(host);

                            if (response == null)
                            {
                                await client.SendAsync("Result", host, "");
                                return;
                            }

                            await UpsertRecordAsync(host, response);
                            await client.SendAsync("Result", host, response.Content);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, $"Whois {host} failed (clientId: {id}).");
                            await client.SendAsync("Result", host, e.Message);
                        }
                        finally
                        {
                            cde.Signal();
                        }
                    },
                    token);
                }

                await cde.WaitAsync();
            });
        }


        private async Task UpsertRecordAsync(string host, WhoisResponse response)
        {
            var doc = await _whois.GetFirstOrDefaultAsync(x => x.Hostname == host);
            var jsonString = JsonConvert.SerializeObject(response, Formatting.None);
            var bson = BsonDocument.Parse(jsonString);

            if (doc != null)
            {
                var update = Builders<Database.Models.Whois>.Update
                    .Set(o => o.Updated, DateTime.UtcNow)
                    .Set(o => o.Result, response.Content)
                    .Set(o => o.ParsedResult, bson);

                await _whois.UpdateOneAsync(x => x.Id == doc.Id, update);
            }
            else
            {
                var now = DateTime.UtcNow;
                doc = new Database.Models.Whois
                {
                    Created = now,
                    Updated = now,
                    Hostname = response.Domain,
                    Result = response.Content,
                    ParsedResult = bson
                };

                await _whois.InsertOneAsync(doc);
            }
        }
    }
}
