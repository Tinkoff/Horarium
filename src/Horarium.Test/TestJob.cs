using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.Test
{
    public class TestJob : IJob<string>
    {
        public async Task Execute(string param)
        {
            await Task.Run(() => { });
        }
    }
}