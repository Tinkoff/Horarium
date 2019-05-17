using System;
using Horarium.Mongo;
using MongoDB.Driver;
using Xunit;

namespace Horarium.Test.Mongo
{
    public class MongoRepositoryTest
    {
        [Fact]
        public void Test()
        {
            var filter = (ExpressionFilterDefinition<JobMongoModel>)Builders<JobMongoModel>.Filter.Where(x =>
                (x.Status == JobStatus.Ready || x.Status == JobStatus.RepeatJob) && x.StartAt < DateTime.UtcNow
                || x.Status == JobStatus.Executing && x.StartedExecuting < DateTime.UtcNow );

            var func = filter.Expression.Compile();
            
            func.Invoke(new JobMongoModel()
            {
                
            })
        }
    }
}