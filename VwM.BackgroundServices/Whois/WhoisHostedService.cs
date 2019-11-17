using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace VwM.BackgroundServices
{
    public class WhoisHostedService : HostedServiceImpl<WhoisHostedService, WhoisTaskQueue>
    {
        public WhoisHostedService(
            IBackgroundTaskQueue<WhoisTaskQueue> queue,
            ILogger<WhoisHostedService> logger) : base(queue, logger) { }
    }
}
