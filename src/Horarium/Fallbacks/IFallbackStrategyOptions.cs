using System;
using Horarium.Builders.JobSequenceBuilder;
using Horarium.Interfaces;

namespace Horarium.Fallbacks
{
    public interface IFallbackStrategyOptions
    {
        void ScheduleFallbackJob<TJob, TJobParam>(TJobParam parameters, Action<IJobSequenceBuilder> fallbackJobConfigure = null)
            where TJob : IJob<TJobParam>;

        void StopExecution();

        void GoToNextJob();
    }
}