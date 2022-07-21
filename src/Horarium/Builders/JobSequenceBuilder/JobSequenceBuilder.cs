using System;
using System.Collections.Generic;
using Horarium.Fallbacks;
using Horarium.Interfaces;

namespace Horarium.Builders.JobSequenceBuilder
{
    internal class JobSequenceBuilder<TJob, TJobParam> : IJobSequenceBuilder where TJob : IJob<TJobParam>
    {
        private readonly Queue<JobMetadata> _jobsQueue = new Queue<JobMetadata>();
        private readonly TimeSpan _globalObsoleteInterval;
        
        private JobMetadata _job;
        
        internal JobSequenceBuilder(TJobParam parameters, TimeSpan globalObsoleteInterval)
        {
            _globalObsoleteInterval = globalObsoleteInterval;
            
            _job = JobBuilderHelpers.GenerateNewJob(typeof(TJob));
            _job.ObsoleteInterval = globalObsoleteInterval;
            _job.JobParam = parameters;

            _jobsQueue.Enqueue(_job);
        }
        
        public IJobSequenceBuilder WithDelay(TimeSpan delay)
        {
            _job.Delay = delay;
            return this;
        }

        public IJobSequenceBuilder AddFallbackConfiguration(Action<IFallbackStrategyOptions> configure)
        {
            var options = new FallbackStrategyOptions(_globalObsoleteInterval);
            if (configure == null)
            {
                return this;
            }
            configure(options);
            
            _job.FallbackStrategyType = options.FallbackStrategyType;
            _job.FallbackJob = options.FallbackJobMetadata;

            return this;
        }

        public IJobSequenceBuilder Next<TNextJob, TNextJobParam>(TNextJobParam parameters)
            where TNextJob : IJob<TNextJobParam>
        {
            _job = JobBuilderHelpers.GenerateNewJob(typeof(TNextJob));
            _job.JobParam = parameters;

            _jobsQueue.Enqueue(_job);

            return this;
        }

        public IJobSequenceBuilder AddRepeatStrategy<TRepeat>() where TRepeat : IFailedRepeatStrategy
        {
            _job.RepeatStrategy = typeof(TRepeat);
            return this;
        }

        public IJobSequenceBuilder MaxRepeatCount(int count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count),"min value is 1");
            }
            _job.MaxRepeatCount = count;
            return this;
        }

        public JobMetadata Build()
        {
            return JobBuilderHelpers.BuildJobsSequence(_jobsQueue, _globalObsoleteInterval);
        }
    }
}