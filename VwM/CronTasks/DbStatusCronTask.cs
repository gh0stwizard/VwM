using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VwM.BackgroundServices;
using VwM.Database.Server;

namespace VwM.CronTasks
{
    public class DbStatusCronTask : ICronTask
    {
        public string Schedule { get; set; } = "*/10 * * * * *"; // each 10 seconds
        public bool IncludeSeconds { get; set; } = true;


        #region ctor
        private readonly ILogger<DbStatusCronTask> _logger;
        private readonly DatabaseStatus _dbStatus;


        public DbStatusCronTask(
            ILogger<DbStatusCronTask> logger,
            DatabaseStatus dbStatus)
        {
            _logger = logger;
            _dbStatus = dbStatus;
        }
        #endregion


        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Start DbStatus cron task.");
            await Task.Run(() => _dbStatus.PingAsync(), cancellationToken);
        }
    }
}
