using System;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.Sample
{
    public class TestJob : IJob<int>
    {
        public async Task Execute(int param)
        {
            Console.WriteLine(param);
            await Task.Run(() => { });
        }
    }
}