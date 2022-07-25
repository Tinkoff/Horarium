using System;
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
            LinkedListNode<Task> linkedListNode;

            lock (_lockObject)
            {
                linkedListNode = _uncompletedTasks.AddLast(task);
            }

            task.ContinueWith((t, state) =>
            {
                lock (_lockObject)
                {
                    _uncompletedTasks.Remove((LinkedListNode<Task>)state);
                }
            }, linkedListNode, CancellationToken.None);
        }

        public async Task WhenAllCompleted(CancellationToken cancellationToken)
        {
            Task[] tasksToAwait;
            lock (_lockObject)
            {
                tasksToAwait = _uncompletedTasks
                    // get rid of fault state, Task.WhenAll shall not throw
                    .Select(x => x.ContinueWith((t) => { }, CancellationToken.None))
                    .ToArray();
            }

            var whenAbandon = Task.Delay(Timeout.Infinite, cancellationToken);
            var whenAllCompleted = Task.WhenAll(tasksToAwait);

            await Task.WhenAny(whenAbandon, whenAllCompleted);

            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException(
                    "Horarium stop timeout is expired. One or many jobs are still running. These jobs may not save their state.",
                    cancellationToken);
        }
    }
}