using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest.Jobs
{
    public class TestObsoleteJob : IJob<int>
    {
        public static TimeSpan JobExecutionTimeSeconds = TimeSpan.FromSeconds(5);
        public static readonly ConcurrentStack<TestObsoleteJob> JobsStack = new ConcurrentStack<TestObsoleteJob>();
        
        public Task Execute(int param)
        {
            JobsStack.Push(this);
            return Task.Delay(JobExecutionTimeSeconds);
        }
    }
}