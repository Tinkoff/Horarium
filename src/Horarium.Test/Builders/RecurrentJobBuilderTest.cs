using System;
using System.Threading.Tasks;
using Cronos;
using Moq;
using Horarium.Builders.Recurrent;
using Horarium.Interfaces;
using Xunit;

namespace Horarium.Test.Builders
{
    public class RecurrentJobBuilderTest
    {
        private readonly Mock<IAdderJobs> _jobsAdderMock = new Mock<IAdderJobs>();
        private static readonly string JobsCron = Cron.SecondInterval(30);
        private readonly TimeSpan _globalObsoleteInterval = TimeSpan.FromMinutes(20);

        [Fact]
        public async Task Schedule_NoPropertiesHaveBeenSet_ShouldSetDefaultAndSchedule()
        {
            // Arrange
            var builder = new RecurrentJobBuilder(_jobsAdderMock.Object, JobsCron, typeof(TestReccurrentJob),
                _globalObsoleteInterval);
            var scheduledJob = new JobMetadata();

            _jobsAdderMock.Setup(a => a.AddRecurrentJob(It.IsAny<JobMetadata>()))
                .Returns(Task.CompletedTask)
                .Callback((JobMetadata job) => scheduledJob = job);

            // Act
            await builder.Schedule();

            // Assert
            Assert.Equal(scheduledJob.JobKey, typeof(TestReccurrentJob).Name);
            _jobsAdderMock.Verify(a => a.AddRecurrentJob(It.IsAny<JobMetadata>()), Times.Once);
            _jobsAdderMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Schedule_CorrectCronPassed_ShouldSetRightStartAtAndSchedule()
        {
            // Arrange
            var builder = new RecurrentJobBuilder(_jobsAdderMock.Object, JobsCron, typeof(TestReccurrentJob),
                _globalObsoleteInterval);
            var scheduledJob = new JobMetadata();

            var parsedCron = CronExpression.Parse(JobsCron, CronFormat.IncludeSeconds);
            var expectedStartAt = parsedCron.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local);

            _jobsAdderMock.Setup(a => a.AddRecurrentJob(It.IsAny<JobMetadata>()))
                .Returns(Task.CompletedTask)
                .Callback((JobMetadata job) => scheduledJob = job);

            // Act
            await builder.Schedule();

            // Assert
            Assert.Equal(scheduledJob.StartAt, expectedStartAt);
            _jobsAdderMock.Verify(a => a.AddRecurrentJob(It.IsAny<JobMetadata>()), Times.Once);
            _jobsAdderMock.VerifyNoOtherCalls();
        }
    }
}