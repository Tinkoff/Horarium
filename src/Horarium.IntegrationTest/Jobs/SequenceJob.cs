using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest.Jobs
{
    public class SequenceJob : IJob<int>
    {
        public static readonly ConcurrentQueue<int> QueueJobs = new ConcurrentQueue<int>();

        public Task Execute(int param)
        {
            QueueJobs.Enqueue(param);
            return Task.CompletedTask;
        }
    }
}