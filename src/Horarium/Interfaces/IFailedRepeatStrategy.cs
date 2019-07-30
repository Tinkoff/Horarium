using System;

namespace Horarium.Interfaces
{
    public interface IFailedRepeatStrategy
    {
        TimeSpan GetNextStartInterval(int countStarted);
    }
}