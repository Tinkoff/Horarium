using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Horarium.Repository;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Horarium.MongoRepository
{
    public class MongoRepository : IJobRepository
    {
        private readonly PropertyInfo[] _jobDbProperties;
        private readonly IMongoClientProvider _mongoClientProvider;

        protected internal MongoRepository(IMongoClientProvider mongoClientProvider)
        {
            _mongoClientProvider = mongoClientProvider;

            _jobDbProperties = typeof(JobDb).GetProperties();
        }

        public async Task<JobDb> GetReadyJob(string machineName, TimeSpan obsoleteTime)
        {
            var collection = _mongoClientProvider.GetCollection<JobDb>();

            var filter = Builders<JobDb>.Filter.Where(x =>
                (x.Status == JobStatus.Ready || x.Status == JobStatus.RepeatJob) && x.StartAt < DateTime.UtcNow
                || x.Status == JobStatus.Executing && x.StartedExecuting < DateTime.UtcNow - obsoleteTime);
            
            var update = Builders<JobDb>.Update
                .Set(x => x.Status, JobStatus.Executing)
                .Set(x => x.ExecutedMachine, machineName)
                .Set(x => x.StartedExecuting, DateTime.UtcNow)
                .Inc(x => x.CountStarted, 1);

            var options = new FindOneAndUpdateOptions<JobDb> {ReturnDocument = ReturnDocument.After};

            return await collection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task AddJob(JobDb job)
        {
            var collection = _mongoClientProvider.GetCollection<JobDb>();
            await collection.InsertOneAsync(job);
        }

        public async Task AddRecurrentJob(JobDb job)
        {
            var collection = _mongoClientProvider.GetCollection<JobDb>();

            var update = Builders<JobDb>.Update
                .Set(x => x.Cron, job.Cron)
                .Set(x => x.StartAt, job.StartAt);

            var needsProperties =
                _jobDbProperties.Where(x => x.Name != nameof(JobDb.Cron) && x.Name != nameof(JobDb.StartAt));

            //Если джоб уже существет апдейтем только 2 поля
            //Если нужно создать, то устанавливаем все остальные поля
            foreach (var jobDbProperty in needsProperties)
            {
                update = update.SetOnInsert(jobDbProperty.Name, jobDbProperty.GetValue(job));
            }

            await collection.UpdateOneAsync(x => x.JobKey == job.JobKey
                                                 && (x.Status == JobStatus.Executing || x.Status == JobStatus.Ready),
                update,
                new UpdateOptions
                {
                    IsUpsert = true
                });
        }

        public async Task AddRecurrentJobSettings(RecurrentJobSettings settings)
        {
            var collection = _mongoClientProvider.GetCollection<RecurrentJobSettings>();

            await collection.ReplaceOneAsync(x => x.JobKey == settings.JobKey, settings,
                new UpdateOptions
                {
                    IsUpsert = true
                });
        }

        public async Task<string> GetCronForRecurrentJob(string jobKey)
        {
            var collection = _mongoClientProvider.GetCollection<RecurrentJobSettings>();

            var recurrentJobSettingsCollection = await collection.FindAsync(x => x.JobKey == jobKey);

            var recurrentJobSettings = recurrentJobSettingsCollection.FirstOrDefault();

            if (recurrentJobSettings == null)
                throw new Exception("Не найдены настройки для рекуррентного джоба");

            return recurrentJobSettings.Cron;
        }

        public async Task RemoveJob(string jobId)
        {
            var collection = _mongoClientProvider.GetCollection<JobDb>();

            await collection.DeleteOneAsync(x => x.JobId == jobId);
        }

        public async Task RepeatJob(string jobId, DateTime startAt, Exception error)
        {
            var collection = _mongoClientProvider.GetCollection<JobDb>();

            var update = Builders<JobDb>.Update
                .Set(x => x.Status, JobStatus.RepeatJob)
                .Set(x => x.StartAt, startAt)
                .Set(x => x.Error, error.Message + error.StackTrace);

            await collection.UpdateOneAsync(x => x.JobId == jobId, update);
        }

        public async Task FailedJob(string jobId, Exception error)
        {
            var collection = _mongoClientProvider.GetCollection<JobDb>();

            var update = Builders<JobDb>.Update
                .Set(x => x.Status, JobStatus.Failed)
                .Set(x => x.Error, error.Message + error.StackTrace);

            await collection.UpdateOneAsync(x => x.JobId == jobId, update);
        }

        public async Task<Dictionary<JobStatus, int>> GetJobStatistic()
        {
            var collection = _mongoClientProvider.GetCollection<JobDb>();
            var result = await collection.Aggregate()
                .Group(x => x.Status, g => new
                {
                    Id = g.Key,
                    Sum = g.Sum(x => 1)
                })
                .ToListAsync();

            var dict = result.ToDictionary(x => x.Id, x => x.Sum);

            foreach (JobStatus jobStatus in Enum.GetValues(typeof(JobStatus)))
            {
                if (!dict.ContainsKey(jobStatus))
                {
                    dict.Add(jobStatus, 0);
                }
            }

            return dict;
        }
    }
}