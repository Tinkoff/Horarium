using System;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest.Jobs.Fallback
{
    public class FallbackMainJob : IJob<int>
    {
        public static int ExecutedCount { get; set; }
        
        public Task Execute(int param)
        {
            ExecutedCount++;

            throw new Exception();
        }
    }
}