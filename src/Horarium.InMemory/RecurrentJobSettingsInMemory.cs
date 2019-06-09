namespace Horarium.InMemory
{
    public class RecurrentJobSettingsInMemory
    {
        public string JobKey { get; private set; }

        public string JobType { get; private set; }
        
        public string Cron { get; private set; }
    }
}