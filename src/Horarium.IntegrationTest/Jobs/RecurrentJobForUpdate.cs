using System.Collections.Concurrent;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest.Jobs
{
    public class RecurrentJobForUpdate : IJobRecurrent
    {
        public static readonly ConcurrentStack<RecurrentJobForUpdate> StackJobs = new ConcurrentStack<RecurrentJobForUpdate>();

        public async Task Execute()
        {
            StackJobs.Push(this);
            await Task.Delay(1000000);
        }
    }
}
