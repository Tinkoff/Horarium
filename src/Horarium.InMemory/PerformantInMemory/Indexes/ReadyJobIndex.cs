using System;
using System.Collections.Generic;
using Horarium.InMemory.PerformantInMemory.Indexes.Comparers;
using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory.Indexes
{
    public class ReadyJobIndex : IAddRemoveIndex
    {
        private readonly SortedSet<JobDb> _startAtIndex = new SortedSet<JobDb>(new StartAtComparer());
        
        private readonly JobKeyIndex _jobKeyIndex = new JobKeyIndex();

        public void Add(JobDb job)
        {
            if (job.Status != JobStatus.Ready) return;
            
            _startAtIndex.Add(job);
            _jobKeyIndex.Add(job);
        }

        public void Remove(JobDb job)
        {
            _startAtIndex.Remove(job);
            _jobKeyIndex.Remove(job);
        }

        public int Count()
        {
            return _startAtIndex.Count;
        }

        public JobDb GetStartAtLessThan(DateTime startAt)
        {
            if (_startAtIndex.Count != 0 && _startAtIndex.Min.StartAt < startAt)
                return _startAtIndex.Min;

            return null;
        }

        public JobDb GetJobKeyEqual(string jobKey)
        {
            return _jobKeyIndex.Get(jobKey);
        }
    }
}