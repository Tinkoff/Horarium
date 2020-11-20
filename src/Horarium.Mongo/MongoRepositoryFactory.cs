using System;
using Horarium.Repository;
using MongoDB.Driver;

namespace Horarium.Mongo
{
    public static class MongoRepositoryFactory
    {
        public static IJobRepository Create(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Connection string is empty");

            var provider = new MongoClientProvider(connectionString);
            return new MongoRepository(provider);
        }
        
        public static IJobRepository Create(MongoUrl mongoUrl)
        {
            if (mongoUrl == null)
                throw new ArgumentNullException(nameof(mongoUrl), "mongoUrl is null");

            var provider = new MongoClientProvider(mongoUrl);
            return new MongoRepository(provider);
        }
    }
}
