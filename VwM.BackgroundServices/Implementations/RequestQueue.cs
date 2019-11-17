using System;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace VwM.BackgroundServices
{
    public class RequestQueue<T, Tdto> : IRequestQueue
        where T: class
    {
        protected readonly ILogger<T> _logger;
        public TimeSpan CleanupTimeSpan { get; protected set; }

        public ConcurrentDictionary<Guid, IEnumerable<Tdto>> Queue { get; } =
            new ConcurrentDictionary<Guid, IEnumerable<Tdto>>();
        protected ConcurrentDictionary<Guid, DateTime> History { get; } =
            new ConcurrentDictionary<Guid, DateTime>();


        public RequestQueue(ILogger<T> logger)
        {
            _logger = logger;
            CleanupTimeSpan = new TimeSpan(0, 1, 0);
        }


        public Guid Add(IEnumerable<Tdto> dtos)
        {
            var id = Guid.NewGuid();

            if (!Queue.TryAdd(id, dtos))
            {
                _logger.LogError($"Failed to add Ping Request {id}.");
                return Guid.Empty;
            }

            if (!History.TryAdd(id, DateTime.UtcNow))
                _logger.LogError($"Failed to add Ping Request History {id}.");

            return id;
        }


        public virtual void Cleanup()
        {
            if (Queue.Count == 0)
                return;

            var now = DateTime.UtcNow;
            var toCleanupIds = History
                .Where(a => now - a.Value >= CleanupTimeSpan)
                .Select(a => a.Key)
                .ToArray();

            _logger.LogDebug($"Cleaning up {toCleanupIds.Length} entries ...");

            foreach (var id in toCleanupIds)
            {
                if (!Queue.TryRemove(id, out _))
                {
                    _logger.LogError($"Failed to remove Ping Request {id}.");
                    continue;
                }

                if (!History.TryRemove(id, out _))
                    _logger.LogError($"Failed to remove Ping Request History {id}.");
            }
        }
    }
}
