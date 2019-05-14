using System;

namespace Horarium.Builders
{
    public interface IDelayedJobBuilder<out TJobBuilder> where TJobBuilder : IJobBuilder
    {
        /// <summary>
        /// Set delay for start this job
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        TJobBuilder WithDelay(TimeSpan delay);
    }
}