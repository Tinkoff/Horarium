using System;
using System.Threading.Tasks;
using Horarium.Mongo;
using MongoDB.Driver;
using Xunit;

namespace Horarium.Test.Mongo
{
    public class MongoRepositoryFactoryTest
    {
        [Fact]
        public void Create_NullConnectionString_Exception()
        {
            string connectionString = null;

            Assert.Throws<ArgumentNullException>(() => MongoRepositoryFactory.Create(connectionString));
        }
        
        [Fact]
        public void Create_NullMongoUrl_Exception()
        {
            MongoUrl mongoUrl = null;

            Assert.Throws<ArgumentNullException>(() => MongoRepositoryFactory.Create(mongoUrl));
        }

        [Fact]
        public async Task Create_WellFormedUrl_AccessMongoLazily()
        {
            const string stubMongoUrl = "mongodb://fake-url:27017/fake_database_name/?serverSelectionTimeoutMs=100";

            var mongoRepository = MongoRepositoryFactory.Create(stubMongoUrl);

            await Assert.ThrowsAsync<TimeoutException>(() => mongoRepository.GetJobStatistic());
        }
    }
}