using System;
using Horarium.Fallbacks;
using Horarium.Repository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Horarium.Mongo
{
    [MongoEntity("horarium.jobs")]
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
                NextJob = NextJob?.ToJobDb(),
                Cron = Cron,
                Delay = Delay,
                ObsoleteInterval = ObsoleteInterval,
                RepeatStrategy = RepeatStrategy,
                MaxRepeatCount = MaxRepeatCount,
                FallbackJob = FallbackJob?.ToJobDb(),
                FallbackStrategyType = FallbackStrategyType
            };
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string JobId { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("JobKey")]
        public string JobKey { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("JobType")]
        public string JobType { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("JobParamType")]
        public string JobParamType { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("JobParam")]
        public string JobParam { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        [BsonElement("Status")]
        public JobStatus Status { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        [BsonElement("CountStarted")]
        public int CountStarted { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("ExecutedMachine")]
        public string ExecutedMachine { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, DateOnly = false, Representation = BsonType.DateTime)]
        [BsonElement("StartedExecuting")]
        public DateTime StartedExecuting { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, DateOnly = false, Representation = BsonType.DateTime)]
        [BsonElement("StartAt")]
        public DateTime StartAt { get; set; }

        [BsonElement("NextJob")]
        public JobMongoModel NextJob { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("Error")]
        public string Error { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("Cron")]
        public string Cron { get; set; }

        [BsonTimeSpanOptions(BsonType.String)]
        [BsonElement("Delay")]
        public TimeSpan? Delay { get; set; }

        [BsonTimeSpanOptions(BsonType.Int64, TimeSpanUnits.Milliseconds)]
        [BsonElement("ObsoleteInterval")]
        public TimeSpan ObsoleteInterval { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("RepeatStrategy")]
        public string RepeatStrategy { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        [BsonElement("MaxRepeatCount")]
        public int MaxRepeatCount { get; set; }
        
        [BsonElement("FallbackJob")]
        public JobMongoModel FallbackJob { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        [BsonElement("FallbackStrategyType")]
        public FallbackStrategyTypeEnum? FallbackStrategyType { get; set; }

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
                ObsoleteInterval = jobDb.ObsoleteInterval,
                RepeatStrategy = jobDb.RepeatStrategy,
                MaxRepeatCount = jobDb.MaxRepeatCount,
                FallbackStrategyType = jobDb.FallbackStrategyType,
                FallbackJob = jobDb.FallbackJob != null ? CreateJobMongoModel(jobDb.FallbackJob) : null,
            };
        }
    }
}