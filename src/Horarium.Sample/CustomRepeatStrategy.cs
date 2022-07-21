using System;
using Horarium.Interfaces;

namespace Horarium.Sample
{
    public class CustomRepeatStrategy : IFailedRepeatStrategy {
        public TimeSpan GetNextStartInterval(int countStarted)
        {
            return TimeSpan.FromSeconds(3);
        }
    }
}