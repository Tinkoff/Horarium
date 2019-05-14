using System;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Horarium.Handlers;
using Horarium.Repository;
using Xunit;

namespace Horarium.Test
{

    public class AdderJobTest
    {
        
        [Fact]
        public async Task AddNewRecurrentJob_Success()
        {
            // Arrange
            var jobRepositoryMock = new Mock<IJobRepository>();
             
            var jobsAdder = new AdderJobs(jobRepositoryMock.Object, new JsonSerializerSettings());
            
            var job = new JobMetadata
            {
                Cron = Cron.SecondInterval(15),
                ObsoleteInterval = TimeSpan.FromMinutes(5),
                JobType = typeof(TestReccurrentJob),
                JobKey = nameof(TestReccurrentJob),
                Status = JobStatus.Ready,
                JobId = Guid.NewGuid().ToString("N"),
                StartAt = DateTime.UtcNow + TimeSpan.FromSeconds(10),
                CountStarted = 0
            };
            
            // Act
            await jobsAdder.AddRecurrentJob(job);

            // Assert
            jobRepositoryMock.Verify(x => x.AddRecurrentJob(It.Is<JobDb>(j => j.Status == job.Status
                                                                                && j.CountStarted == job.CountStarted
                                                                                && j.JobKey == job.JobKey
                                                                                && j.Cron == job.Cron
                                                                                && j.JobId == job.JobId
                )), Times.Once);
        }
    }
}