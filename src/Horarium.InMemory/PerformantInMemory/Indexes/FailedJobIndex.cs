using System.Collections.Generic;
using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory.Indexes
{
    public class FailedJobIndex : IAddRemoveIndex
    {
        private readonly Dictionary<string, JobDb> _index = new Dictionary<string, JobDb>();
        
        public void Add(JobDb job)
        {
            if (job.Status != JobStatus.Failed) return;
            
            _index.Add(job.JobId, job);
        }

        public void Remove(JobDb job)
        {
            _index.Remove(job.JobId);
        }

        public int Count()
        {
            return _index.Count;
        }
    }
}