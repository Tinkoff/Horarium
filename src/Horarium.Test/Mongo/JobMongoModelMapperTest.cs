using System;
using Horarium.Mongo;
using Horarium.Repository;
using Xunit;

namespace Horarium.Test.Mongo
{
    public class JobMongoModelMapperTest
    {
        [Fact]
        public void CreateJobMongoModel_AllFieldsSuccessMap()
        {
            var jobDb = new JobDb
            {
                JobType = "Horarium.TestJob, Horarium",
                JobParamType = "System.Int32, System.Private.CoreLib",
                JobParam = "437",
                Status = JobStatus.Ready,
                CountStarted = 0,
                NextJob = null,
                Cron = "* * * * * *",
                Delay = TimeSpan.FromSeconds(5)
            };

            var jobMongoModel = JobMongoModel.CreateJobMongoModel(jobDb);

            Assert.Equal("Horarium.TestJob, Horarium", jobMongoModel.JobType);
            Assert.Equal("System.Int32, System.Private.CoreLib", jobMongoModel.JobParamType);
            Assert.Equal("437", jobMongoModel.JobParam);
            Assert.Equal(JobStatus.Ready, jobMongoModel.Status);
            Assert.Equal(0, jobMongoModel.CountStarted);
            Assert.Null(jobMongoModel.NextJob);
            Assert.Equal("* * * * * *", jobMongoModel.Cron);
            Assert.Equal(TimeSpan.FromSeconds(5), jobMongoModel.Delay);
        }
        
        [Fact]
        public void ToJobDb_AllFieldsSuccessMap()
        {
            var jobMongoModel = new JobMongoModel
            {
                JobType = "Horarium.TestJob, Horarium",
                JobParamType = "System.Int32, System.Private.CoreLib",
                JobParam = "437",
                Status = JobStatus.Ready,
                CountStarted = 0,
                NextJob = null,
                Cron = "* * * * * *",
                Delay = TimeSpan.FromSeconds(5)
            };

            var jobDb = jobMongoModel.ToJobDb();

            Assert.Equal("Horarium.TestJob, Horarium", jobDb.JobType);
            Assert.Equal("System.Int32, System.Private.CoreLib", jobDb.JobParamType);
            Assert.Equal("437", jobDb.JobParam);
            Assert.Equal(JobStatus.Ready, jobDb.Status);
            Assert.Equal(0, jobDb.CountStarted);
            Assert.Null(jobDb.NextJob);
            Assert.Equal("* * * * * *", jobDb.Cron);
            Assert.Equal(TimeSpan.FromSeconds(5), jobDb.Delay);
        }
    }
}