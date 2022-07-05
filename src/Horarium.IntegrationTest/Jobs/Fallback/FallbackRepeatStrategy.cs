using System;
using Horarium.Fallbacks;
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