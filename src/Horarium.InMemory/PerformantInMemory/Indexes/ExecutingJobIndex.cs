using System;
using System.Collections.Generic;
using Horarium.InMemory.PerformantInMemory.Indexes.Comparers;
using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory.Indexes
{
    public class ExecutingJobIndex : IAddRemoveIndex
    {
        private readonly SortedSet<JobDb> _startedExecutingIndex = new SortedSet<JobDb>(new StartedExecutingComparer());
        
        private readonly JobKeyIndex _jobKeyIndex = new JobKeyIndex();
        
        public void Add(JobDb job)
        {
            if (job.Status != JobStatus.Executing) return;

            _startedExecutingIndex.Add(job);
            _jobKeyIndex.Add(job);
        }

        public void Remove(JobDb job)
        {
            _startedExecutingIndex.Remove(job);
            _jobKeyIndex.Remove(job);
        }

        public int Count()
        {
            return _startedExecutingIndex.Count;
        }

        public JobDb GetJobKeyEqual(string jobKey)
        {
            return _jobKeyIndex.Get(jobKey);
        }

        public JobDb GetStartedExecutingLessThan(DateTime startedExecuting)
        {
            if (_startedExecutingIndex.Count != 0 && _startedExecutingIndex.Min.StartAt < startedExecuting)
                return _startedExecutingIndex.Min;

            return null;
        }
    }
}