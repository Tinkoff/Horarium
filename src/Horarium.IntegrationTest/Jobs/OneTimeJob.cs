using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest.Jobs
{
    public class OneTimeJob : IJob<int>
    {
        public static bool Runned;

        public async Task Execute(int param)
        {
            Runned = true;
        }
    }
}