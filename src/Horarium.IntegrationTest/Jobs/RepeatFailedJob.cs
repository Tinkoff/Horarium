using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest.Jobs
{
    public class RepeatFailedJob : IJob<string>, IAllRepeatesIsFailed
    {
        public static readonly ConcurrentQueue<DateTime> ExecutingTime = new ConcurrentQueue<DateTime>();

        public static bool RepeatIsOk;
        
        public Task Execute(string param)
        {
            ExecutingTime.Enqueue(DateTime.Now);
            
            throw new Exception();
        }

        public Task FailedEvent(object param, Exception ex)
        {
            RepeatIsOk = true;
            return Task.CompletedTask;
        }
    }
}