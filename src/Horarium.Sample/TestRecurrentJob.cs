using System;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.Sample
{
    public class TestRecurrentJob : IJobRecurrent
    {
        public Task Execute()
        {
            Console.WriteLine("Run -" + DateTime.Now);
            return Task.CompletedTask;
        }
    }
}