using System;
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

        private readonly List<IAddRemoveIndex> _indexes;

        public JobsStorage()
        {
            _indexes = new List<IAddRemoveIndex>
            {
                _readyJobIndex,
                _executingJobIndex,
                _readyJobIndex,
                _failedJobIndex
            };
        }

        public void Add(JobDb job)
        {
            _jobs.Add(job.JobId, job);

            _indexes.ForEach(x => x.Add(job));
        }

        public void Remove(string jobId)
        {
            if (!_jobs.TryGetValue(jobId, out var job)) return;

            Remove(job);
        }

        public void Remove(JobDb job)
        {
            _jobs.Remove(job.JobId);
            
            _indexes.ForEach(x => x.Remove(job));
        }

        public Dictionary<JobStatus, int> GetStatistics()
        {
            return new Dictionary<JobStatus, int>
            {
                {JobStatus.Ready, _readyJobIndex.Count()},
                {JobStatus.Executing, _executingJobIndex.Count()},
                {JobStatus.RepeatJob, _repeatJobIndex.Count()},
                {JobStatus.Failed, _failedJobIndex.Count()}
            };
        }

        public JobDb GetById(string jobId)
        {
            if (!_jobs.TryGetValue(jobId, out var job)) return null;

            return job;
        }

        public JobDb FindRecurrentJobToUpdate(string jobKey)
        {
            return _readyJobIndex.GetJobKeyEqual(jobKey) ?? _executingJobIndex.GetJobKeyEqual(jobKey);
        }

        public JobDb FindReadyJob(TimeSpan obsoleteTime)
        {
            var now = DateTime.UtcNow;

            return _readyJobIndex.GetStartAtLessThan(now) ??
                   _repeatJobIndex.GetStartAtLessThan(now) ??
                   _executingJobIndex.GetStartedExecutingLessThan(now - obsoleteTime);
        }
    }
}