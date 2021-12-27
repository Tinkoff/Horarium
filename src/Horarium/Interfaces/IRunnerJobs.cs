using System.Threading;
using System.Threading.Tasks;

namespace Horarium.Interfaces
{
    public interface IRunnerJobs
    {
        void Start();

        /// <summary>
        /// Stops scheduling next jobs and awaits currently running jobs.
        /// If <paramref name="stopCancellationToken"/> is cancelled, then abandons running jobs.
        /// </summary>
        Task Stop(CancellationToken stopCancellationToken);
    }
}