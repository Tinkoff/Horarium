using Horarium.MongoRepository;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Horarium.Repository
{
    [MongoEntity("scheduler.recurrentJobSettings")]
    public class RecurrentJobSettings
    {
        public static RecurrentJobSettings CreatedRecurrentJobSettings(RecurrentJobSettingsMetadata jobMetadata, JsonSerializerSettings jsonSerializerSettings)
        {
            return new RecurrentJobSettings
            {
                JobKey = jobMetadata.JobKey,
                JobType = jobMetadata.JobType.AssemblyQualifiedNameWithoutVersion(),
                Cron = jobMetadata.Cron
            };
        }
        
        [BsonId]
        public string JobKey { get; private set; }

        public string JobType { get; private set; }
        
        public string Cron { get; private set; }
    }
}
