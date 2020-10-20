using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.Handlers
{
    public class UncompletedTaskList : IUncompletedTaskList
    {
        private readonly LinkedList<Task> _uncompletedTasks = new LinkedList<Task>();
        private readonly object _lockObject = new object();

        public int Count
        {
            get
            {
                lock (_lockObject) return _uncompletedTasks.Count;
            }
        }

        public void Add(Task task)
        {
            lock (_lockObject)
            {
                _uncompletedTasks.AddLast(task);
            }

            task.ContinueWith((t, state) =>
            {
                lock (_lockObject)
                {
                    _uncompletedTasks.Remove(task);
                }
            }, CancellationToken.None, TaskScheduler.Default);
        }

        public async Task WhenAllCompleted()
        {
            Task[] tasksToAwait;
            lock (_lockObject)
            {
                tasksToAwait = _uncompletedTasks.ToArray();
            }

            try
            {
                await Task.WhenAll(tasksToAwait);
            }
            catch
            {
                // We just want to have all task completed by now.
                // Any possible exceptions must be handled in jobs.
            }
        }
    }
}