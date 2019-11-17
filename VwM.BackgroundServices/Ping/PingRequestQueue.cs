using Microsoft.Extensions.Logging;
using VwM.BackgroundServices.Ping;

namespace VwM.BackgroundServices
{
    public sealed class PingRequestQueue : RequestQueue<PingRequestQueue, PingDto>
    {
        public PingRequestQueue(ILogger<PingRequestQueue> logger) : base(logger) { }
    }
}
