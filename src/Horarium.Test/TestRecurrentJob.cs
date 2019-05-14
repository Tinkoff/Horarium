using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.Test
{
    public class TestReccurrentJob : IJobRecurrent
    {
        public Task Execute()
        {
            return Task.CompletedTask;
        }
    }
}