using System;
using System.Collections.Generic;
using Horarium.InMemory.Indexes.Comparers;
using Horarium.Repository;

namespace Horarium.InMemory.Indexes
{
    public class JobKeyIndex : IAddRemoveIndex
    {
        private readonly Dictionary<string, SortedSet<JobDb>> _index = new Dictionary<string, SortedSet<JobDb>>();

        public void Add(JobDb job)
        {
            if (string.IsNullOrEmpty(job.JobKey)) return;

            if (!_index.TryGetValue(job.JobKey, out var set))
                _index[job.JobKey] = new SortedSet<JobDb>(new JobIdComparer()) {job};
            else
                set.Add(job);
        }

        public void Remove(JobDb job)
        {
            if (string.IsNullOrEmpty(job.JobKey)) return;
            
            if (!_index.TryGetValue(job.JobKey, out var set)) return;

            set.Remove(job);
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public JobDb Get(string jobKey)
        {
            if (!_index.TryGetValue(jobKey, out var set)) return null;

            if (set.Count != 0) return set.Min;

            return null;
        }
    }
}