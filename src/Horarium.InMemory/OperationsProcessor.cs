using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Horarium.Repository;

namespace Horarium.InMemory
{
    internal class OperationsProcessor
    {
        private readonly Channel<BaseWrapper> _channel = Channel.CreateUnbounded<BaseWrapper>();

        public OperationsProcessor()
        {
            Task.Run(ProcessQueue);
        }

        private async Task ProcessQueue()
        {
            while (await _channel.Reader.WaitToReadAsync())
            {
                while (_channel.Reader.TryRead(out var operation))
                {
                    operation.Execute();
                }
            }
        }

        public Task Execute(Action command)
        {
            var wrapped = new CommandWrapper(command);
            _channel.Writer.TryWrite(wrapped);

            return wrapped.Task;
        }

        public Task<JobDb> Execute(Func<JobDb> query)
        {
            var wrapped = new QueryWrapper(query);
            _channel.Writer.TryWrite(wrapped);

            return wrapped.Task;
        }
        
        private abstract class BaseWrapper
        {
            protected readonly TaskCompletionSource<JobDb> CompletionSource = 
                new TaskCompletionSource<JobDb>(TaskCreationOptions.RunContinuationsAsynchronously);

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
                CompletionSource.SetResult(_query()?.Copy());
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