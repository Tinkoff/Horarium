using System;
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
    }
}