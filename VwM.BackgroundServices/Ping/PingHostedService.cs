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
    public class PingHostedService : HostedServiceImpl<PingHostedService, PingTaskQueue>
    {
        public PingHostedService(
            IBackgroundTaskQueue<PingTaskQueue> queue,
            ILogger<PingHostedService> logger) : base(queue, logger) { }
    }
}
