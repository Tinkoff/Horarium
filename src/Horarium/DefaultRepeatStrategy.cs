using System;
using Horarium.Interfaces;

namespace Horarium
{
    public class DefaultRepeatStrategy :IFailedRepeatStrategy
    {
        public TimeSpan GetNextStartInterval(int countStarted)
        {
            const int increaseRepeat = 10;
            
            return TimeSpan.FromMinutes(increaseRepeat * countStarted);
        }
    }
}