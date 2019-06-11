using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Horarium.InMemory.PerformantInMemory.Indexes;
using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory
{
    public class PerformantInMemoryRepository : IJobRepository
    {
        private readonly object _syncRoot = new object();
        
        private readonly JobsStorage _storage = new JobsStorage();
            
        public Task<JobDb> GetReadyJob(string machineName, TimeSpan obsoleteTime)
        {
            throw new NotImplementedException();
        }

        public Task AddJob(JobDb job)
        {
            lock (_syncRoot)
            {
                _storage.Add(job);
            }

            return Task.CompletedTask;
        }

        public Task FailedJob(string jobId, Exception error)
        {
            throw new NotImplementedException();
        }

        public Task RemoveJob(string jobId)
        {
            throw new NotImplementedException();
        }

        public Task RepeatJob(string jobId, DateTime startAt, Exception error)
        {
            throw new NotImplementedException();
        }

        public Task AddRecurrentJob(JobDb job)
        {
            lock (_syncRoot)
            {
                var foundJob = _storage
            }
        }

        public Task AddRecurrentJobSettings(RecurrentJobSettings settings)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetCronForRecurrentJob(string jobKey)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<JobStatus, int>> GetJobStatistic()
        {
            throw new NotImplementedException();
        }
    }
}