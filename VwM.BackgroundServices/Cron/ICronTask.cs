using System.Threading;
using System.Threading.Tasks;

namespace VwM.BackgroundServices
{
    public interface ICronTask
    {
        string Schedule { get; set; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
