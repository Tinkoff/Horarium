using System.Collections.Concurrent;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest.Jobs
{
    public class TestJob : IJob<int>
    {
        public static readonly ConcurrentStack<int> StackJobs = new ConcurrentStack<int>();

        public async Task Execute(int param)
        {
            StackJobs.Push(param);
            await Task.Delay(30);
        }
    }
}