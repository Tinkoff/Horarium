using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horarium.Fallbacks;
using Horarium.Interfaces;

namespace Horarium.Builders.Parameterized
{
    internal class ParameterizedJobBuilder<TJob, TJobParam> : JobBuilder, IParameterizedJobBuilder
        where TJob : IJob<TJobParam>
    {
        private readonly IAdderJobs _adderJobs;
        private readonly TimeSpan _globalObsoleteInterval;
        private readonly Queue<JobMetadata> _jobsQueue = new Queue<JobMetadata>();

        internal ParameterizedJobBuilder(IAdderJobs adderJobs, TJobParam parameters, TimeSpan globalObsoleteInterval)
            : base(typeof(TJob))
        {
            _adderJobs = adderJobs;
            _globalObsoleteInterval = globalObsoleteInterval;
            Job.ObsoleteInterval = globalObsoleteInterval;

            Job.JobParam = parameters;

            _jobsQueue.Enqueue(Job);
        }

        public IParameterizedJobBuilder WithDelay(TimeSpan delay)
        {
            Job.Delay = delay;
            return this;
        }

        public IParameterizedJobBuilder ObsoleteAfter(TimeSpan obsoleteInterval)
        {
            Job.ObsoleteInterval = obsoleteInterval;
            return this;
        }

        public IParameterizedJobBuilder Next<TNextJob, TNextJobParam>(TNextJobParam parameters)
            where TNextJob : IJob<TNextJobParam>
        {
            GenerateNewJob<TNextJob>();
            Job.JobParam = parameters;

            _jobsQueue.Enqueue(Job);

            return this;
        }

        public IParameterizedJobBuilder AddFallbackConfiguration(Action<IFallbackStrategyOptions> configure)
        {
            var options = new FallbackStrategyOptions(_globalObsoleteInterval);
            if (configure == null)
            {
                return this;
            }
            configure(options);
            
            Job.FallbackStrategyType = options.FallbackStrategyType;
            Job.FallbackJob = options.FallbackJobMetadata;

            return this;
        }

        public IParameterizedJobBuilder AddRepeatStrategy<TRepeat>() where TRepeat : IFailedRepeatStrategy
        {
            Job.RepeatStrategy = typeof(TRepeat);
            return this;
        }

        public IParameterizedJobBuilder MaxRepeatCount(int count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count),"min value is 1");
            }
            Job.MaxRepeatCount = count;
            return this;
        }

        public override Task Schedule()
        {
            var job = JobBuilderHelpers.BuildJobsSequence(_jobsQueue, _globalObsoleteInterval);

            return _adderJobs.AddEnqueueJob(job);
        }
    }
}