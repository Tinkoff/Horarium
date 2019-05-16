using Horarium.Repository;
using MongoDB.Bson.Serialization.Attributes;

namespace Horarium.Mongo
{
    [MongoEntity("scheduler.recurrentJobSettings")]
    public class RecurrentJobSettingsMongo
    {
        public static RecurrentJobSettingsMongo Create(RecurrentJobSettings jobSettings)
        {
            return new RecurrentJobSettingsMongo
            {
                JobKey = jobSettings.JobKey,
                JobType = jobSettings.JobType,
                Cron = jobSettings.Cron
            };
        }
        
        [BsonId]
        public string JobKey { get; private set; }

        public string JobType { get; private set; }
        
        public string Cron { get; private set; }
    }
}
