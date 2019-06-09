using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Horarium.Repository;

namespace Horarium.InMemory
{
    public class InMemoryRepository : IJobRepository
    {   
        private readonly ConcurrentDictionary<string, JobInMemoryModel> _jobsStorage = 
            new ConcurrentDictionary<string, JobInMemoryModel>();
        
        private readonly ConcurrentDictionary<string, RecurrentJobSettings> _recurrentJobSettingsStorage = 
            new ConcurrentDictionary<string, RecurrentJobSettings>();
        
        public Task<JobDb> GetReadyJob(string machineName, TimeSpan obsoleteTime)
        {
            bool Filter(JobDb x) =>
                (x.Status == JobStatus.Ready || x.Status == JobStatus.RepeatJob) &&
                x.StartAt < DateTime.UtcNow ||
                x.Status == JobStatus.Executing && x.StartedExecuting < DateTime.UtcNow - obsoleteTime;

            void Update(JobDb x)
            {
                x.Status = JobStatus.Executing;
                x.ExecutedMachine = machineName;
                x.StartedExecuting = DateTime.UtcNow;
                x.CountStarted++;
            }

            return Task.FromResult(UpdateOneAtomic(Filter, Update));
        }

        public Task AddJob(JobDb job)
        {
            _jobsStorage.TryAdd(job.JobId, JobInMemoryModel.CreateJobInMemoryModel(job));

            return Task.CompletedTask;
        }

        public Task FailedJob(string jobId, Exception error)
        {
            if (!_jobsStorage.TryGetValue(jobId, out var jobWrapper))
                return Task.CompletedTask;

            lock (jobWrapper.SyncRoot)
            {
                jobWrapper.Job.Status = JobStatus.Failed;
                jobWrapper.Job.Error = error.Message + error.StackTrace;
            }

            return Task.CompletedTask;
        }

        public Task RemoveJob(string jobId)
        {
            _jobsStorage.TryRemove(jobId, out var _);

            return Task.CompletedTask;
        }

        public Task RepeatJob(string jobId, DateTime startAt, Exception error)
        {
            if (!_jobsStorage.TryGetValue(jobId, out var jobWrapper))
                return Task.CompletedTask;

            lock (jobWrapper.SyncRoot)
            {
                jobWrapper.Job.Status = JobStatus.RepeatJob;
                jobWrapper.Job.StartAt = startAt;
                jobWrapper.Job.Error = error.Message + error.StackTrace;
            }

            return Task.CompletedTask;
        }

        public Task AddRecurrentJob(JobDb job)
        {
            bool Filter(JobDb x) =>
                x.JobKey == job.JobKey &&
                (x.Status == JobStatus.Executing || x.Status == JobStatus.Ready);

            void Update(JobDb x)
            {
                x.Cron = job.Cron;
                x.StartAt = job.StartAt;
            }

            var result = UpdateOneAtomic(Filter, Update);
            if (result != null) return Task.CompletedTask;

            var copy = JobInMemoryModel.CopyJob(job);
            Update(copy);
            
            return AddJob(copy);
        }

        public Task AddRecurrentJobSettings(RecurrentJobSettings settings)
        {
            _recurrentJobSettingsStorage.AddOrUpdate(settings.JobKey, settings, 
                (s, jobSettings) => settings);
            
            return Task.CompletedTask;
        }

        public Task<string> GetCronForRecurrentJob(string jobKey)
        {
            if (!_recurrentJobSettingsStorage.TryGetValue(jobKey, out var settings))
                throw new Exception("Не найдены настройки для рекуррентного джоба");

            return Task.FromResult(settings.Cron);
        }

        public Task<Dictionary<JobStatus, int>> GetJobStatistic()
        {
            var dict = new Dictionary<JobStatus, int>();

            return Task.FromResult(dict);
        }

        private JobDb UpdateOneAtomic(Func<JobDb, bool> filter, Action<JobDb> update)
        {
            foreach (var kvp in _jobsStorage)
            {
                var jobWrapper = kvp.Value;

                if (filter(jobWrapper.Job))
                {
                    lock (jobWrapper.SyncRoot)
                    {
                        if (filter(jobWrapper.Job))
                        {
                            update(jobWrapper.Job);
                            return JobInMemoryModel.CopyJob(jobWrapper.Job);
                        }
                    }
                }
            }

            return null;
        }
    }
}