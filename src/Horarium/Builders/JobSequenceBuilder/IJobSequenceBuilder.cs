using System;
using Horarium.Fallbacks;
using Horarium.Interfaces;

namespace Horarium.Builders.JobSequenceBuilder
{
    public interface IJobSequenceBuilder
    {
        /// <summary>
        /// Create next job, it run after previous job
        /// </summary>
        /// <param name="parameters"></param>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TJobParam"></typeparam>
        /// <returns></returns>
        IJobSequenceBuilder Next<TJob, TJobParam>(TJobParam parameters) where TJob : IJob<TJobParam>;

        /// <summary>
        /// Add custom failed repeat strategy for job
        /// </summary>
        /// <typeparam name="TRepeat"></typeparam>
        /// <returns></returns>
        IJobSequenceBuilder AddRepeatStrategy<TRepeat>() where TRepeat : IFailedRepeatStrategy;
        
        /// <summary>
        /// Set custom max failed repeat count
        /// </summary>
        /// <param name="count">min value is 1, it's mean this job start only one time</param>
        /// <returns></returns>
        IJobSequenceBuilder MaxRepeatCount(int count);

        /// <summary>
        /// Add custom fallback configuration for job
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        IJobSequenceBuilder AddFallbackConfiguration(Action<IFallbackStrategyOptions> configure);
        
        /// <summary>
        /// Set delay for start this job
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        IJobSequenceBuilder WithDelay(TimeSpan delay);
    }
}