using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest.Jobs
{
    public class OneTimeJob : IJob<int>
    {
        public static bool Run;

        public Task Execute(int param)
        {
            Run = true;
            return Task.CompletedTask;
        }
    }
}