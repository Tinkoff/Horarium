using System;
using System.Collections.Generic;
using Horarium.InMemory.PerformantInMemory.Indexes.Comparers;
using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory.Indexes
{
    public class ReadyJobIndex
    {
        private readonly SortedSet<JobDb> _startAtIndex = new SortedSet<JobDb>(new StartAtComparer());

        private readonly Dictionary<string, SortedSet<JobDb>> _jobKeyIndex = new Dictionary<string, SortedSet<JobDb>>();

        public void Add(JobDb job)
        {
            _startAtIndex.Add(job);

            if (!_jobKeyIndex.ContainsKey(job.JobKey))
                _jobKeyIndex[job.JobKey] = new SortedSet<JobDb>(new JobIdComparer()) {job};
            else
                _jobKeyIndex[job.JobKey].Add(job);
        }

        public void Remove(JobDb job)
        {
            _startAtIndex.Remove(job);
            _jobKeyIndex[job.JobKey].Remove(job);
        }

        public JobDb GetStartAtLessThan(DateTime startAt)
        {
            if (_startAtIndex.Count != 0 && _startAtIndex.Min.StartAt < startAt)
                return _startAtIndex.Min;

            return null;
        }

        public JobDb GetJobKeyEqual(string jobKey)
        {
            if (!_jobKeyIndex.TryGetValue(jobKey, out var index)) return null;

            if (index.Count != 0) return index.Min;
            
            return null;
        }
    }
}