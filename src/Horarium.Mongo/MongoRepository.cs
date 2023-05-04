using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Horarium.Builders;
using Horarium.Repository;
using MongoDB.Driver;

namespace Horarium.Mongo
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
            var collection = _mongoClientProvider.GetCollection<JobMongoModel>();

            var filter = Builders<JobMongoModel>.Filter.Where(x =>
                (x.Status == JobStatus.Ready || x.Status == JobStatus.RepeatJob) && x.StartAt < DateTime.UtcNow
                || x.Status == JobStatus.Executing && x.StartedExecuting < DateTime.UtcNow - obsoleteTime);

            var update = Builders<JobMongoModel>.Update
                .Set(x => x.Status, JobStatus.Executing)
                .Set(x => x.ExecutedMachine, machineName)
                .Set(x => x.StartedExecuting, DateTime.UtcNow)
                .Inc(x => x.CountStarted, 1);

            var options = new FindOneAndUpdateOptions<JobMongoModel> {ReturnDocument = ReturnDocument.After};

            var result = await collection.FindOneAndUpdateAsync(filter, update, options);

            return result?.ToJobDb();
        }

        public async Task AddJob(JobDb job)
        {
            var collection = _mongoClientProvider.GetCollection<JobMongoModel>();
            await collection.InsertOneAsync(JobMongoModel.CreateJobMongoModel(job));
        }

        public async Task AddRecurrentJob(JobDb job)
        {
            var collection = _mongoClientProvider.GetCollection<JobMongoModel>();

            var update = Builders<JobMongoModel>.Update
                .Set(x => x.Cron, job.Cron)
                .Set(x => x.StartAt, job.StartAt);

            var needsProperties = _jobDbProperties.Where(x =>
                x.Name != nameof(JobMongoModel.Cron) && x.Name != nameof(JobMongoModel.StartAt));

            //Если джоб уже существет апдейтем только 2 поля
            //Если нужно создать, то устанавливаем все остальные поля
            foreach (var jobDbProperty in needsProperties)
            {
                update = update.SetOnInsert(jobDbProperty.Name, jobDbProperty.GetValue(job));
            }

            await collection.UpdateOneAsync(
                x => x.JobKey == job.JobKey && (x.Status == JobStatus.Executing || x.Status == JobStatus.Ready),
                update,
                new UpdateOptions
                {
                    IsUpsert = true
                });
        }

        public async Task AddRecurrentJobSettings(RecurrentJobSettings settings)
        {
            var collection = _mongoClientProvider.GetCollection<RecurrentJobSettingsMongo>();

            await collection.ReplaceOneAsync(
                x => x.JobKey == settings.JobKey,
                RecurrentJobSettingsMongo.Create(settings),
                new UpdateOptions
                {
                    IsUpsert = true
                });
        }

        public async Task<string> GetCronForRecurrentJob(string jobKey)
        {
            var collection = _mongoClientProvider.GetCollection<RecurrentJobSettingsMongo>();

            var recurrentJobSettingsCollection = await collection.FindAsync(x => x.JobKey == jobKey);

            var recurrentJobSettings = recurrentJobSettingsCollection.FirstOrDefault();

            if (recurrentJobSettings == null)
                throw new Exception("Не найдены настройки для рекуррентного джоба");

            return recurrentJobSettings.Cron;
        }

        public async Task RemoveJob(string jobId)
        {
            var collection = _mongoClientProvider.GetCollection<JobMongoModel>();

            await collection.DeleteOneAsync(x => x.JobId == jobId);
        }

        public async Task RescheduleRecurrentJob(string jobId, DateTime startAt, Exception error)
        {
            var collection = _mongoClientProvider.GetCollection<JobMongoModel>();

            var update = Builders<JobMongoModel>.Update
                .Set(x => x.StartAt, startAt)
                .Set(x => x.Status, error != null ? JobStatus.Ready : JobStatus.Failed)
                .Set(x => x.Error, error != null ? error.Message + ' ' + error.StackTrace : null);

            if (error == null)
            {
                await collection.UpdateOneAsync(x => x.JobId == jobId, update);

                return;
            }
            
            var job = await collection
                .Find(Builders<JobMongoModel>.Filter.Where(x => x.JobId == jobId))
                .FirstOrDefaultAsync();

            job.JobId = JobBuilderHelpers.GenerateNewJobId();
            job.StartAt = startAt;
            job.Status = JobStatus.Ready;

            await collection.BulkWriteAsync(new List<WriteModel<JobMongoModel>>
            {
                new UpdateOneModel<JobMongoModel>(Builders<JobMongoModel>.Filter.Where(x => x.JobId == jobId), update),
                new InsertOneModel<JobMongoModel>(job)
            });
        }

        public async Task RepeatJob(string jobId, DateTime startAt, Exception error)
        {
            var collection = _mongoClientProvider.GetCollection<JobMongoModel>();

            var update = Builders<JobMongoModel>.Update
                .Set(x => x.Status, JobStatus.RepeatJob)
                .Set(x => x.StartAt, startAt)
                .Set(x => x.Error, error.Message + ' ' + error.StackTrace);

            await collection.UpdateOneAsync(x => x.JobId == jobId, update);
        }

        public async Task FailedJob(string jobId, Exception error)
        {
            var collection = _mongoClientProvider.GetCollection<JobMongoModel>();

            var update = Builders<JobMongoModel>.Update
                .Set(x => x.Status, JobStatus.Failed)
                .Set(x => x.Error, error.Message + ' ' + error.StackTrace);

            await collection.UpdateOneAsync(x => x.JobId == jobId, update);
        }

        public async Task<Dictionary<JobStatus, int>> GetJobStatistic()
        {
            var collection = _mongoClientProvider.GetCollection<JobMongoModel>();
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