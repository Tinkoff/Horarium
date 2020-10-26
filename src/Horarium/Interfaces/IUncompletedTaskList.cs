using System;
using System.Threading;
using System.Threading.Tasks;

namespace Horarium.Interfaces
{
    /// <summary>
    /// Keeps references to a task until it is completed.
    /// </summary>
    public interface IUncompletedTaskList
    {
        /// <summary>
        /// Adds new task to monitor.
        /// </summary>
        void Add(Task task);

        /// <summary>
        /// Returns task that will complete (with success) when all currently running tasks complete or fail.
        /// </summary>
        /// <param name="cancellationToken">If cancelled, throws <see cref="OperationCanceledException"/> immediately.</param>
        Task WhenAllCompleted(CancellationToken cancellationToken);
    }
}