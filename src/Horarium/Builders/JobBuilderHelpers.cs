using System;
using System.Collections.Generic;
using System.Linq;

namespace Horarium.Builders
{
    public static class JobBuilderHelpers
    {
        public static JobMetadata GenerateNewJob(Type jobType)
        {
            return new JobMetadata
            {
                JobId = Guid.NewGuid().ToString("N"),
                JobType = jobType,
                Status = JobStatus.Ready,
                CountStarted = 0
            }; 
        }

        public static JobMetadata BuildJobsSequence(Queue<JobMetadata> jobsQueue, TimeSpan globalObsoleteInterval)
        {
            var job = jobsQueue.Dequeue();

            FillWithDefaultIfNecessary(job, globalObsoleteInterval);
            var previous = job;

            while (jobsQueue.Any())
            {
                previous.NextJob = jobsQueue.Dequeue();
                previous = previous.NextJob;
                FillWithDefaultIfNecessary(previous, globalObsoleteInterval);
            }

            return job;
        }
        
        private static void FillWithDefaultIfNecessary(JobMetadata job, TimeSpan globalObsoleteInterval)
        {
            job.Delay = job.Delay ?? TimeSpan.Zero;
            job.StartAt = DateTime.UtcNow + job.Delay.Value;

            job.ObsoleteInterval = job.ObsoleteInterval == default(TimeSpan)
                ? globalObsoleteInterval
                : job.ObsoleteInterval;
        }
    }
}