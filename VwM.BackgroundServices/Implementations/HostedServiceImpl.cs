using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VwM.BackgroundServices
{
    public class HostedServiceImpl<T, Tqueue> : BackgroundService
        where T: class
        where Tqueue : class
    {
        private readonly ILogger<T> _logger;


        public IBackgroundTaskQueue<Tqueue> TaskQueue { get; }
        public List<Task> Jobs { get; } = new List<Task>();


        #region ctor
        public HostedServiceImpl(IBackgroundTaskQueue<Tqueue> taskQueue, ILogger<T> logger)
        {
            TaskQueue = taskQueue;
            _logger = logger;
        }
        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Ping Hosted Service is running.");

            stoppingToken.Register(() =>
                _logger.LogInformation("Ping Hosted Service is stopping."));

            Jobs.Add(BackgroundProcessing(stoppingToken));

            await Task.WhenAll(Jobs);
        }


        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occurred executing {0}.", nameof(workItem));
                }
            }
        }


        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
            _logger.LogInformation("Ping Hosted Service has been stop.");
        }
    }
}
