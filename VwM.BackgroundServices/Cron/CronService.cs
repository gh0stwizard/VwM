using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using VwM.BackgroundServices.Cron;

/*
 * Based on https://blog.maartenballiauw.be/post/2017/08/01/building-a-scheduled-cache-updater-in-aspnet-core-2.html
 */
namespace VwM.BackgroundServices
{
    public class CronService : BackgroundService
    {
        private readonly ILogger<CronService> _logger;
        private readonly List<CronTaskWrapper> _scheduledTasks = new List<CronTaskWrapper>();


        public CronService(ILogger<CronService> logger, IEnumerable<ICronTask> scheduledTasks)
        {
            _logger = logger;

            var referenceTime = DateTime.UtcNow;

            foreach (var scheduledTask in scheduledTasks)
            {
                _scheduledTasks.Add(new CronTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(scheduledTask.Schedule),
                    Task = scheduledTask,
                    NextRunTime = referenceTime
                });
            }
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }


        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.UtcNow;

            var tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime)).ToList();

            foreach (var taskThatShouldRun in tasksThatShouldRun)
            {
                taskThatShouldRun.Increment();

                await taskFactory.StartNew(async () =>
                {
                    try
                    {
                        await taskThatShouldRun.Task.ExecuteAsync(cancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Failed to run Cron Task.");
                    }
                },
                cancellationToken);
            }
        }
    }
}
