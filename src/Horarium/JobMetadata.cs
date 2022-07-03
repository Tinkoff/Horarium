using System;
using Horarium.Fallbacks;

namespace Horarium
{
    public class JobMetadata
    {
        public string JobId { get; set; }

        public Type JobType { get; set; }

        public object JobParam { get; set; }

        public JobStatus Status { get; set; }

        public int CountStarted { get; set; }

        public string ExecutedMachine { get; set; }

        public DateTime StartedExecuting { get; set; }

        public DateTime StartAt { get; set; }

        public JobMetadata NextJob { get; set; }

        public string JobKey { get; set; }
        
        public string Cron { get; set; }

        public TimeSpan? Delay { get; set; }
        
        public TimeSpan ObsoleteInterval { get; set; }
        
        public Type RepeatStrategy { get; set; }
        
        public int MaxRepeatCount { get; set; }
        
        public FallbackStrategyTypeEnum? FallbackStrategyType { get; set; }
        
        public JobMetadata FallbackJob { get; set; }
    }
}