using System;
using Horarium.Mongo;
using Horarium.Repository;

namespace Horarium.IntegrationTest
{
    public class IntegrationTestBase
    {
        protected const string IntegrationTestCollection = "IntegrationTestCollection";
        private static readonly string DatabaseNameMongo = "IntegrationTestScheduler" + Guid.NewGuid().ToString();

        private readonly string _connectionMongo =
            $"mongodb://{Environment.GetEnvironmentVariable("MONGO_ADDRESS") ?? "localhost"}:27017/{DatabaseNameMongo}";


        protected HorariumServer CreateHorariumServer()
        {
            var dataBase = Environment.GetEnvironmentVariable("DataBase");

            IJobRepository jobRepository;

            switch (dataBase)
            {
                case "MongoDB":
                    jobRepository = MongoRepositoryFactory.Create(_connectionMongo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataBase), dataBase, null);
            }

            var horarium = new HorariumServer(jobRepository);
            horarium.Start();

            return horarium;
        }
    }
}