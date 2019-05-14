using System;

namespace Horarium
{
    public class RecurrentJobSettingsMetadata
    {
        public RecurrentJobSettingsMetadata(string jobKey, Type jobType, string cron)
        {
            JobKey = jobKey;
            JobType = jobType;
            Cron = cron;
        }

        public string JobKey { get; private set; }

        public Type JobType { get; private set; }

        public string Cron { get; private set; }
    }
}
