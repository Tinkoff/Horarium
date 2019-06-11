using System;
using System.Collections.Generic;
using Horarium.InMemory.PerformantInMemory.Indexes.Comparers;
using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory.Indexes
{
    public class RepeatJobIndex : IAddRemoveIndex
    {
        private readonly SortedSet<JobDb> _startAtIndex = new SortedSet<JobDb>(new StartAtComparer());
        
        public void Add(JobDb job)
        {
            if (job.Status != JobStatus.RepeatJob) return;
            
            _startAtIndex.Add(job);
        }

        public void Remove(JobDb job)
        {
            _startAtIndex.Remove(job);
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
    }
}