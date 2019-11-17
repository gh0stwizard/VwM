using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Globalization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Nito.AsyncEx;
using VwM.BackgroundServices;
using VwM.Extensions;
using VwM.BackgroundServices.Ping;

namespace VwM.Hubs
{
    public class PingHub : GenericHub<PingHub, PingRequestQueue, PingTaskQueue>
    {
        private readonly System.Net.IPAddress zeroIP = new System.Net.IPAddress(0);


        public PingHub(
            ILogger<PingHub> logger,
            IStringLocalizer<PingHub> localizer,
            PingRequestQueue queue,
            IBackgroundTaskQueue<PingTaskQueue> taskQueue) : base(logger, localizer, queue, taskQueue)
        {
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

            if (!_queue.Queue.TryRemove(queueId, out IEnumerable<PingDto> dtos))
            {
                _logger.LogError($"Failed remove Ping Request {queueId} for client {id}.");
                await client.SendAsync("Exception", "Internal error (queue).");
                return;
            }

            _logger.LogInformation($"User {login} has started Ping Request {queueId} with {dtos.Count()} host(s).");

            var culture = await GetCultureAsync(id, language);

            _tasks.QueueTask(async token =>
            {
                if (token.IsCancellationRequested)
                    return;

                Thread.CurrentThread.CurrentCulture =
                Thread.CurrentThread.CurrentUICulture = culture;

                var cde = new AsyncCountdownEvent(dtos.Count());

                foreach (var dto in dtos)
                {
                    var ping = new Ping();
                    var state = new PingResult(dto.Name, dto.Hostname, cde, client);
                    ping.PingCompleted += new PingCompletedEventHandler(PingHandler);
                    ping.SendAsync(dto.Hostname, state);
                }

                await cde.WaitAsync();
            });
        }


        private void PingHandler(object sender, PingCompletedEventArgs e)
        {
            var state = (PingResult)e.UserState;

            if (e.Cancelled)
                state.Result = _lcz["Canceled"];
            else if (e.Error != null)
                state.Result = e.Error.GetLastException().Message.ToString();
            else if (e.Reply.Address.Equals(zeroIP))
                state.Result = e.Reply.Status.ToString(); // FIXME: TimedOut and ... ?
            else
            {
                state.Result = e.Reply.RoundtripTime.ToString();
                state.Hostname = e.Reply.Address.ToString();
            }

            state.Client.SendAsync("Result", state);
            state.CountdownEvent.Signal();
        }


        private class PingResult : PingDto
        {
            public string Result { get; set; }
            internal readonly AsyncCountdownEvent CountdownEvent;
            internal readonly IClientProxy Client;


            public PingResult(
                string name,
                string hostname,
                AsyncCountdownEvent cde,
                IClientProxy client) : base(name, hostname)
            {
                CountdownEvent = cde;
                Client = client;
            }
        }
    }
}
