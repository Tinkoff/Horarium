using Horarium.Repository;

namespace Horarium.InMemory
{
    internal class JobInMemoryModel
    {
        public object SyncRoot { get; set; }

        public JobDb Job { get; set; }

        public static JobInMemoryModel CreateJobInMemoryModel(JobDb job)
        {
            return new JobInMemoryModel
            {
                SyncRoot = new object(),
                Job = CopyJob(job)
            };
        }

        public static JobDb CopyJob(JobDb source)
        {
            return new JobDb
            {
                JobKey = source.JobKey,
                JobId = source.JobId,
                Status = source.Status,
                JobType = source.JobType,
                JobParamType = source.JobParamType,
                JobParam = source.JobParam,
                CountStarted = source.CountStarted,
                StartedExecuting = source.StartedExecuting,
                ExecutedMachine = source.ExecutedMachine,
                StartAt = source.StartAt,
                NextJob = source.NextJob == null ? null : CopyJob(source.NextJob),
                Cron = source.Cron,
                Delay = source.Delay,
                ObsoleteInterval = source.ObsoleteInterval
            };
        }
    }
}