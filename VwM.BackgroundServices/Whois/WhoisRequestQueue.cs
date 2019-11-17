using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using VwM.BackgroundServices.Whois;

namespace VwM.BackgroundServices
{
    public sealed class WhoisRequestQueue : RequestQueue<WhoisRequestQueue, WhoisDto>
    {
        public WhoisRequestQueue(ILogger<WhoisRequestQueue> logger) : base(logger) { }
    }
}
