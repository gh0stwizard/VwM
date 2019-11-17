using System;
using System.Threading;
using System.Threading.Tasks;

namespace VwM.BackgroundServices
{
    public interface IBackgroundTaskQueue<T>
        where T : class
    {
        void QueueTask(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
