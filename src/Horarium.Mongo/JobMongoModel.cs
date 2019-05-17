using System;
using Horarium.Repository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Horarium.Mongo
{
    [MongoEntity("scheduler.jobs")]
    public class JobMongoModel
    {
        public JobDb ToJobDb()
        {
            return new JobDb
            {
                JobKey = JobKey,
                JobId = JobId,
                Status = Status,
                JobType = JobType,
                JobParamType = JobParamType,
                JobParam = JobParam,
                CountStarted = CountStarted,
                StartedExecuting = StartedExecuting,
                ExecutedMachine = ExecutedMachine,
                StartAt = StartAt,
                NextJob =
                    NextJob?.ToJobDb(),
                Cron = Cron,
                Delay = Delay,
                ObsoleteInterval = ObsoleteInterval
            };
        }

        [BsonId] public string JobId { get; set; }

        public string JobKey { get; set; }

        public string JobType { get; set; }

        public string JobParamType { get; set; }

        public string JobParam { get; set; }

        public JobStatus Status { get; set; }

        public int CountStarted { get; set; }

        public string ExecutedMachine { get; set; }

        public DateTime StartedExecuting { get; set; }

        public DateTime StartAt { get; set; }

        public JobMongoModel NextJob { get; set; }

        public string Error { get; set; }

        public string Cron { get; set; }

        public TimeSpan? Delay { get; set; }

        [BsonTimeSpanOptions(BsonType.Int64, TimeSpanUnits.Milliseconds)]
        public TimeSpan ObsoleteInterval { get; set; }

        public static JobMongoModel CreateJobMongoModel(JobDb jobDb)
        {
            return new JobMongoModel
            {
                JobId = jobDb.JobId,
                JobKey = jobDb.JobKey,
                Status = jobDb.Status,
                CountStarted = jobDb.CountStarted,
                StartedExecuting = jobDb.StartedExecuting,
                ExecutedMachine = jobDb.ExecutedMachine,
                JobType = jobDb.JobType,
                JobParam = jobDb.JobParam,
                JobParamType = jobDb.JobParamType,
                StartAt = jobDb.StartAt,
                NextJob = jobDb.NextJob != null ? CreateJobMongoModel(jobDb.NextJob) : null,
                Cron = jobDb.Cron,
                Delay = jobDb.Delay,
                ObsoleteInterval = jobDb.ObsoleteInterval
            };
        }
    }
}