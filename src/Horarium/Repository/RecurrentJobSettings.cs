
namespace Horarium.Repository
{
    public class RecurrentJobSettings
    {
        public static RecurrentJobSettings CreatedRecurrentJobSettings(RecurrentJobSettingsMetadata jobMetadata)
        {
            return new RecurrentJobSettings
            {
                JobKey = jobMetadata.JobKey,
                JobType = jobMetadata.JobType.AssemblyQualifiedNameWithoutVersion(),
                Cron = jobMetadata.Cron
            };
        }
        
        public string JobKey { get; private set; }

        public string JobType { get; private set; }
        
        public string Cron { get; private set; }
    }
}
