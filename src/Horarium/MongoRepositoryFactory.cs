using System;
using Horarium.MongoRepository;
using Horarium.Repository;

namespace Horarium
{
    public static class MongoRepositoryFactory
    {
        public static IJobRepository Create(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException($"Не указана строка подключения.");

            var provider = new MongoClientProvider(connectionString);
            return new MongoRepository.MongoRepository(provider);
        }
    }
}
