using System;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.Sample
{
    public class FailedTestJob : IJob<int>
    {
        public Task Execute(int param)
        {
            Console.WriteLine($"Failed job executed with param {param}");
            throw new Exception();
        }
    }
}