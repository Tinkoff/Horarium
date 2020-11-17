using Horarium.Repository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Horarium.Mongo
{
    [MongoEntity("horarium.recurrentJobSettings")]
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
        [BsonRepresentation(BsonType.String)]
        public string JobKey { get; private set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("JobType")]
        public string JobType { get; private set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("Cron")]
        public string Cron { get; private set; }
    }
}
