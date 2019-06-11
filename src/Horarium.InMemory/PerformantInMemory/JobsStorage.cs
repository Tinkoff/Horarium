using System.Collections.Generic;
using Horarium.InMemory.PerformantInMemory.Indexes;
using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory
{
    public class JobsStorage
    {
        private readonly Dictionary<string, JobDb> _jobs = new Dictionary<string, JobDb>();
        
        private readonly ReadyJobIndex _readyJobIndex = new ReadyJobIndex();
        private readonly ExecutingJobIndex _executingJobIndex = new ExecutingJobIndex();
        private readonly RepeatJobIndex _repeatJobIndex = new RepeatJobIndex();
        private readonly FailedJobIndex _failedJobIndex = new FailedJobIndex();

        public void Add(JobDb job)
        {
            _jobs.Add(job.JobId, job);
        }

        public void Remove(string jobId)
        {
            if (!_jobs.TryGetValue(jobId, out var job)) return;

            _jobs.Remove(jobId);
        }

        public JobDb FindRecurrentJobToUpdate(string jobKey)
        {
            return _readyJobIndex.GetJobKeyEqual(jobKey) ?? _executingJobIndex
        }
    }
}