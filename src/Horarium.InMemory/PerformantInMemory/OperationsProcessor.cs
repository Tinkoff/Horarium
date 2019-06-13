using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory
{
    internal class OperationsProcessor
    {
        private readonly ConcurrentQueue<BaseWrapper> _queue = new ConcurrentQueue<BaseWrapper>();

        public OperationsProcessor()
        {
            ProcessQueue();
        }

        private void ProcessQueue()
        {
            while (_queue.TryDequeue(out var operation))
            {
                operation.Execute();
            }

            Task.Delay(TimeSpan.FromMilliseconds(10))
                .ContinueWith(x => ProcessQueue());
        }

        public Task Execute(Action command)
        {
            var wrapped = new CommandWrapper(command);
            _queue.Enqueue(wrapped);

            return wrapped.Task;
        }

        public Task<JobDb> Execute(Func<JobDb> query)
        {
            var wrapped = new QueryWrapper(query);
            _queue.Enqueue(wrapped);

            return wrapped.Task;
        }
        
        private abstract class BaseWrapper
        {
            protected readonly TaskCompletionSource<JobDb> CompletionSource = new TaskCompletionSource<JobDb>();

            public abstract void Execute();

            public Task<JobDb> Task => CompletionSource.Task;
        }

        private class QueryWrapper : BaseWrapper
        {
            private readonly Func<JobDb> _query;

            public QueryWrapper(Func<JobDb> query)
            {
                _query = query;
            }
            
            public override void Execute()
            {
                CompletionSource.SetResult(_query());
            }
        }

        private class CommandWrapper : BaseWrapper
        {
            private readonly Action _command;

            public CommandWrapper(Action command)
            {
                _command = command;
            }
        
            public override void Execute()
            {
                _command();
            
                CompletionSource.SetResult(null);
            }
        }
    }
}