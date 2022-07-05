using System;
using Horarium.Builders.Parameterized;
using Horarium.Fallbacks;
using Horarium.Interfaces;

namespace Horarium.Builders.Fallback
{
    public interface IFallbackJobBuilder
    {
        /// <summary>
        /// Create next job, it run after previous job
        /// </summary>
        /// <param name="parameters"></param>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TJobParam"></typeparam>
        /// <returns></returns>
        IFallbackJobBuilder Next<TJob, TJobParam>(TJobParam parameters) where TJob : IJob<TJobParam>;

        /// <summary>
        /// Add custom failed repeat strategy for job
        /// </summary>
        /// <typeparam name="TRepeat"></typeparam>
        /// <returns></returns>
        IFallbackJobBuilder AddRepeatStrategy<TRepeat>() where TRepeat : IFailedRepeatStrategy;
        
        /// <summary>
        /// Set custom max failed repeat count
        /// </summary>
        /// <param name="count">min value is 1, it's mean this job start only one time</param>
        /// <returns></returns>
        IFallbackJobBuilder MaxRepeatCount(int count);
        
        /// <summary>
        /// Set delay for start this job
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        IFallbackJobBuilder WithDelay(TimeSpan delay);
        
        /// <summary>
        /// Add custom fallback configuration for job
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        IFallbackJobBuilder AddFallbackConfiguration(Action<IFallbackStrategyOptions> configure);
    }
}