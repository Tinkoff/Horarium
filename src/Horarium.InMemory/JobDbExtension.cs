using Horarium.Repository;

namespace Horarium.InMemory
{
    internal static class JobDbExtension
    {
        public static JobDb Copy(this JobDb source)
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
                NextJob = source.NextJob?.Copy(),
                Cron = source.Cron,
                Delay = source.Delay,
                ObsoleteInterval = source.ObsoleteInterval,
                RepeatStrategy = source.RepeatStrategy,
                MaxRepeatCount = source.MaxRepeatCount
                
            };
        }
    }
}