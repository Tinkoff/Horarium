using System;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest.Jobs.Fallback
{
    public class FallbackRepeatStrategy : IFailedRepeatStrategy
    {
        public TimeSpan GetNextStartInterval(int countStarted)
        {
            return TimeSpan.FromSeconds(5);
        }
    }
}