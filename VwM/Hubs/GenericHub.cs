using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using VwM.BackgroundServices;

namespace VwM.Hubs
{
    [Authorize]
    public class GenericHub<T, Trq, Ttq> : BaseHub
        where T : class
        where Trq : class
        where Ttq : class
    {
        protected readonly ILogger<T> _logger;
        protected readonly IStringLocalizer<T> _lcz;
        protected readonly Trq _queue;
        protected readonly IBackgroundTaskQueue<Ttq> _tasks;


        public GenericHub(
            ILogger<T> logger,
            IStringLocalizer<T> localizer,
            Trq queue,
            IBackgroundTaskQueue<Ttq> taskQueue)
        {
            _logger = logger;
            _queue = queue;
            _tasks = taskQueue;
            _lcz = localizer;
        }


        public async ValueTask<CultureInfo> GetCultureAsync(string clientId, string language)
        {
            var culture = Thread.CurrentThread.CurrentUICulture;

            try
            {
                culture = CultureInfo.GetCultureInfo(language);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Failed to identify culture for client {clientId}.");
                await Clients.Client(clientId).SendAsync("Exception", "Bad culture");
            }

            return culture;
        }
    }
}
