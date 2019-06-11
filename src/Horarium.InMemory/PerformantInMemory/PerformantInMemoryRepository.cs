using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory
{
    public class PerformantInMemoryRepository : IJobRepository
    {
        private readonly object _syncRoot = new object();

        private readonly JobsStorage _storage = new JobsStorage();

        private readonly ConcurrentDictionary<string, RecurrentJobSettings> _settingsStorage =
            new ConcurrentDictionary<string, RecurrentJobSettings>();

        public Task<JobDb> GetReadyJob(string machineName, TimeSpan obsoleteTime)
        {
            lock (_syncRoot)
            {
                var job = _storage.FindReadyJob(obsoleteTime);
                if (job == null) return null;

                _storage.Remove(job.JobId);

                job.Status = JobStatus.Executing;
                job.ExecutedMachine = machineName;
                job.StartedExecuting = DateTime.UtcNow;
                job.CountStarted++;

                _storage.Add(job);

                return Task.FromResult(job);
            }
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
            lock (_syncRoot)
            {
                var job = _storage.GetById(jobId);
                if (job == null) return Task.CompletedTask;

                _storage.Remove(job);

                job.Status = JobStatus.Failed;
                job.Error = error.Message + error.StackTrace;

                _storage.Add(job);
            }

            return Task.CompletedTask;
        }

        public Task RemoveJob(string jobId)
        {
            lock (_syncRoot)
            {
                _storage.Remove(jobId);
            }

            return Task.CompletedTask;
        }

        public Task RepeatJob(string jobId, DateTime startAt, Exception error)
        {
            lock (_syncRoot)
            {
                var job = _storage.GetById(jobId);
                if (job == null) return Task.CompletedTask;

                _storage.Remove(job);

                job.Status = JobStatus.RepeatJob;
                job.StartAt = startAt;
                job.Error = error.Message + error.StackTrace;

                _storage.Add(job);
            }

            return Task.CompletedTask;
        }

        public Task AddRecurrentJob(JobDb job)
        {
            lock (_syncRoot)
            {
                var foundJob = _storage.FindRecurrentJobToUpdate(job.JobKey) ?? job;

                _storage.Remove(foundJob);

                foundJob.Cron = job.Cron;
                foundJob.StartAt = job.StartAt;

                _storage.Add(foundJob);
            }

            return Task.CompletedTask;
        }

        public Task AddRecurrentJobSettings(RecurrentJobSettings settings)
        {
            _settingsStorage.AddOrUpdate(settings.JobKey, settings, (_, __) => settings);

            return Task.CompletedTask;
        }

        public Task<string> GetCronForRecurrentJob(string jobKey)
        {
            if (!_settingsStorage.TryGetValue(jobKey, out var settings))
                throw new Exception($"Settings for recurrent job (jobKey = {jobKey}) aren't found");

            return Task.FromResult(settings.Cron);
        }

        public Task<Dictionary<JobStatus, int>> GetJobStatistic()
        {
            lock (_syncRoot)
            {
                return Task.FromResult(_storage.GetStatistics());
            }
        }
    }
}