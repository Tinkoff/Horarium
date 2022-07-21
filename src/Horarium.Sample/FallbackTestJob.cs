using System;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.Sample
{
    public class FallbackTestJob : IJob<int>
    {
        public Task Execute(int param)
        {
            Console.WriteLine($"Fallback job executed with param {param}");
            return Task.CompletedTask;
        }
    }
}