

using System;

namespace Horarium.IntegrationTest
{
    public class IntegrationTestBase
    {
        protected const string IntegrationTestCollection = "IntegrationTestCollection";
        protected const string DatabaseNameMongo = "IntegrationTestScheduler";
        
        protected readonly string ConnectionMongo =
            $"mongodb://{Environment.GetEnvironmentVariable("MONGO_ADDRESS")??"localhost"}:27017/{DatabaseNameMongo}";

        
    }
}
