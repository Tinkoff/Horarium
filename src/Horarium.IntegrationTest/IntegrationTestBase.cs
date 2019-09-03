using System;
using System.Threading.Tasks;
using Horarium.Mongo;
using Horarium.Repository;

namespace Horarium.IntegrationTest
{
    public class IntegrationTestBase
    {
        protected const string IntegrationTestCollection = "IntegrationTestCollection";
        private string DatabaseNameMongo => "IntegrationTestHorarium" + Guid.NewGuid();
        
        private string ConnectionMongo => $"mongodb://{Environment.GetEnvironmentVariable("MONGO_ADDRESS") ?? "localhost"}:27017/{DatabaseNameMongo}";
        
        protected HorariumServer CreateHorariumServer()
        {
            var dataBase = Environment.GetEnvironmentVariable("DataBase");

            IJobRepository jobRepository;

            switch (dataBase)
            {
                case "MongoDB":
                    jobRepository = MongoRepositoryFactory.Create(ConnectionMongo);
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
