using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VwM.BackgroundServices;

namespace VwM.CronTasks
{
    public class CleanupPingCronTask : ICronTask
    {
        public string Schedule { get; set; } = "* */5 * * *";
        public bool IncludeSeconds { get; set; } = false;

        private readonly ILogger<CleanupPingCronTask> _logger;
        private readonly PingRequestQueue _queue;


        public CleanupPingCronTask(ILogger<CleanupPingCronTask> logger, PingRequestQueue queue)
        {
            _logger = logger;
            _queue = queue;
        }


        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Start Ping Queue Cleanup task.");
            await Task.Run(() => _queue.Cleanup(), cancellationToken);
        }
    }
}
