using System;
using Horarium.Fallbacks;
using Newtonsoft.Json;

namespace Horarium.Repository
{
    public class JobDb
    {
        public static JobDb CreatedJobDb(JobMetadata jobMetadata, JsonSerializerSettings jsonSerializerSettings)
        {
            return new JobDb
            {
                JobKey = jobMetadata.JobKey,
                JobId = jobMetadata.JobId,
                Status = jobMetadata.Status,
                JobType = jobMetadata.JobType.AssemblyQualifiedNameWithoutVersion(),
                JobParamType = jobMetadata.JobParam?.GetType().AssemblyQualifiedNameWithoutVersion(),
                JobParam = jobMetadata.JobParam?.ToJson(jobMetadata.JobParam.GetType(), jsonSerializerSettings),
                CountStarted = jobMetadata.CountStarted,
                StartedExecuting = jobMetadata.StartedExecuting,
                ExecutedMachine = jobMetadata.ExecutedMachine,
                StartAt = jobMetadata.StartAt,
                NextJob =
                    jobMetadata.NextJob != null ? CreatedJobDb(jobMetadata.NextJob, jsonSerializerSettings) : null,
                Cron = jobMetadata.Cron,
                Delay = jobMetadata.Delay,
                ObsoleteInterval = jobMetadata.ObsoleteInterval,
                RepeatStrategy = jobMetadata.RepeatStrategy?.AssemblyQualifiedNameWithoutVersion(),
                MaxRepeatCount = jobMetadata.MaxRepeatCount,
                FallbackStrategyType = jobMetadata.FallbackStrategyType,
                FallbackJob = jobMetadata.FallbackJob != null ? CreatedJobDb(jobMetadata.FallbackJob, jsonSerializerSettings) : null,
            };
        }

        public string JobId { get; set; }

        public string JobKey { get; set; }

        public string JobType { get; set; }

        public string JobParamType { get; set; }

        public string JobParam { get; set; }

        public JobStatus Status { get; set; }

        public int CountStarted { get; set; }

        public string ExecutedMachine { get; set; }

        public DateTime StartedExecuting { get; set; }

        public DateTime StartAt { get; set; }

        public JobDb NextJob { get; set; }

        public string Error { get; set; }

        public string Cron { get; set; }

        public TimeSpan? Delay { get; set; }

        public TimeSpan ObsoleteInterval { get; set; }

        public string RepeatStrategy { get; set; }

        public int MaxRepeatCount { get; set; }
        
        public FallbackStrategyTypeEnum? FallbackStrategyType { get; set; }
        
        public JobDb FallbackJob { get; set; }

        public JobMetadata ToJob(JsonSerializerSettings jsonSerializerSettings)
        {
            return new JobMetadata
            {
                JobId = JobId,
                JobKey = JobKey,
                Status = Status,
                CountStarted = CountStarted,
                StartedExecuting = StartedExecuting,
                ExecutedMachine = ExecutedMachine,
                JobType = Type.GetType(JobType, true),
                JobParam = JobParam?.FromJson(Type.GetType(JobParamType), jsonSerializerSettings),
                StartAt = StartAt,
                NextJob = NextJob?.ToJob(jsonSerializerSettings),
                Cron = Cron,
                Delay = Delay,
                ObsoleteInterval = ObsoleteInterval,
                RepeatStrategy = string.IsNullOrEmpty(RepeatStrategy) ? null : Type.GetType(RepeatStrategy, true),
                MaxRepeatCount = MaxRepeatCount,
                FallbackStrategyType = FallbackStrategyType,
                FallbackJob = FallbackJob?.ToJob(jsonSerializerSettings)
            };
        }
    }
}