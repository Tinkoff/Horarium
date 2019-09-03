using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest.Jobs
{
    public class RecurrentJob : IJobRecurrent
    {
        public static readonly ConcurrentQueue<DateTime> ExecutingTime = new ConcurrentQueue<DateTime>();
        
        public Task Execute()
        {
            ExecutingTime.Enqueue(DateTime.Now);
            
            return Task.CompletedTask;
        }
    }
}