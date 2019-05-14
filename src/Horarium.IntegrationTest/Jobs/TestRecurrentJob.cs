using System.Collections.Concurrent;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest.Jobs
{
    public class TestRecurrentJob : IJobRecurrent
    {
        public static readonly ConcurrentStack<TestRecurrentJob> StackJobs = new ConcurrentStack<TestRecurrentJob>();

        public async Task Execute()
        {
            StackJobs.Push(this);
            await Task.Delay(1000000);
        }
    }
}
