using System;
using Horarium.Builders.Fallback;
using Horarium.Builders.Parameterized;
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
        
        public void CreateFallbackJob<TJob, TJobParam>(TJobParam parameters, Action<IFallbackJobBuilder> fallbackJobConfigure = null) where TJob : IJob<TJobParam>
        {
            FallbackStrategyType = FallbackStrategyTypeEnum.ScheduleFallbackJob;
            
            var builder = new FallbackJobBuilder<TJob, TJobParam>(parameters, _globalObsoleteInterval);
            fallbackJobConfigure?.Invoke(builder);
            
            FallbackJobMetadata = builder.BuildJob();
        }

        public void StopExecution()
        {
            FallbackStrategyType = FallbackStrategyTypeEnum.StopExecuting;
        }

        public void GoNext()
        {
            FallbackStrategyType = FallbackStrategyTypeEnum.GoNext;
        }
    }
}