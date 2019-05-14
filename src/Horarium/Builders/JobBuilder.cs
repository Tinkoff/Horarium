using System;
using System.Threading.Tasks;

namespace Horarium.Builders
{
    internal abstract class JobBuilder : IJobBuilder
    {
        protected JobMetadata Job;
        
        protected JobBuilder(Type jobType)
        {
            GenerateNewJob(jobType);
        }

        public abstract Task Schedule();

        protected void GenerateNewJob<TJob>()
        {
            GenerateNewJob(typeof(TJob));
        }

        private void GenerateNewJob(Type jobType)
        {
            Job = new JobMetadata
            {
                JobId = Guid.NewGuid().ToString("N"),
                JobType = jobType,
                Status = JobStatus.Ready,
                CountStarted = 0
            }; 
        }
    }
}