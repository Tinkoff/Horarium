using System;
using Horarium.Builders.JobSequenceBuilder;
using Horarium.Interfaces;

namespace Horarium.Fallbacks
{
    internal class FallbackStrategyOptions : IFallbackStrategyOptions
    {
        private readonly TimeSpan _globalObsoleteInterval;
        
        public FallbackStrategyTypeEnum? FallbackStrategyType { get; private set; }
        public JobMetadata FallbackJobMetadata { get; private set; }

        public FallbackStrategyOptions(TimeSpan globalObsoleteInterval)
        {
            _globalObsoleteInterval = globalObsoleteInterval;
        }
        
        public void ScheduleFallbackJob<TJob, TJobParam>(TJobParam parameters, Action<IJobSequenceBuilder> fallbackJobConfigure = null) where TJob : IJob<TJobParam>
        {
            FallbackStrategyType = FallbackStrategyTypeEnum.ScheduleFallbackJob;
            
            var builder = new JobSequenceBuilder<TJob, TJobParam>(parameters, _globalObsoleteInterval);
            fallbackJobConfigure?.Invoke(builder);
            
            FallbackJobMetadata = builder.Build();
        }

        public void StopExecution()
        {
            FallbackStrategyType = FallbackStrategyTypeEnum.StopExecution;
        }

        public void GoToNextJob()
        {
            FallbackStrategyType = FallbackStrategyTypeEnum.GoToNextJob;
        }
    }
}