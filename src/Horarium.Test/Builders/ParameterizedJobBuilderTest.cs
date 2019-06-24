using System;
using System.Threading.Tasks;
using Horarium.Builders.Parameterized;
using Moq;
using Horarium.Interfaces;
using Xunit;

namespace Horarium.Test.Builders
{
    public class ParameterizedJobBuilderTest
    {
        private readonly Mock<IAdderJobs> _jobsAdderMock = new Mock<IAdderJobs>();
        private readonly TimeSpan _globalObsoleteInterval = TimeSpan.FromMinutes(20);

        [Fact]
        public async Task Schedule_NoPropertiesHaveBeenPassed_ShouldScheduleWithDefault()
        {
            // Arrange
            var builder =
                new ParameterizedJobBuilder<TestJob, string>(_jobsAdderMock.Object, "HALLO", _globalObsoleteInterval);
            var scheduledJob = new JobMetadata();

            _jobsAdderMock.Setup(a => a.AddEnqueueJob(It.IsAny<JobMetadata>()))
                .Returns(Task.CompletedTask)
                .Callback((JobMetadata job) => scheduledJob = job);

            // Act
            await builder.Schedule();

            // Assert
            Assert.Equal(scheduledJob.StartAt, DateTime.UtcNow, TimeSpan.FromMilliseconds(500));
            Assert.Null(scheduledJob.RepeatStrategy);
            Assert.Equal(0, scheduledJob.MaxRepeatCount);
            _jobsAdderMock.Verify(a => a.AddEnqueueJob(It.IsAny<JobMetadata>()), Times.Once);
            _jobsAdderMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Schedule_SetWithDelayAndObsoleteInterval_ShouldScheduleWithDelay()
        {
            // Arrange
            var delay = TimeSpan.FromSeconds(15);
            var scheduledJob = new JobMetadata();

            var builder =
                new ParameterizedJobBuilder<TestJob, string>(_jobsAdderMock.Object, "HALLO", _globalObsoleteInterval)
                    .WithDelay(delay);

            _jobsAdderMock.Setup(a => a.AddEnqueueJob(It.IsAny<JobMetadata>()))
                .Returns(Task.CompletedTask)
                .Callback((JobMetadata job) => scheduledJob = job);

            // Act
            await builder.Schedule();

            // Assert
            Assert.Equal(scheduledJob.StartAt, DateTime.UtcNow + delay, TimeSpan.FromMilliseconds(500));
            Assert.Equal(scheduledJob.Delay, delay);
            Assert.Equal(scheduledJob.ObsoleteInterval, _globalObsoleteInterval);
            _jobsAdderMock.Verify(a => a.AddEnqueueJob(It.IsAny<JobMetadata>()), Times.Once);
            _jobsAdderMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Schedule_ManyJobs_ShouldScheduleCorrectly()
        {
            // Arrange
            var builder = new ParameterizedJobBuilder<TestJob, string>(_jobsAdderMock.Object, "HALLO",
                _globalObsoleteInterval);
            var scheduledJob = new JobMetadata();

            var secondJobDelay = TimeSpan.FromSeconds(27);

            var thirdJobDelay = TimeSpan.FromMinutes(20);

            _jobsAdderMock.Setup(a => a.AddEnqueueJob(It.IsAny<JobMetadata>()))
                .Returns(Task.CompletedTask)
                .Callback((JobMetadata job) => scheduledJob = job);

            // Act
            await builder
                .Next<TestJob, string>("HALLO2")
                .WithDelay(secondJobDelay)
                .Next<TestJob, string>("HALLO3")
                .WithDelay(thirdJobDelay)
                .Schedule();

            // Assert
            var firstJob = scheduledJob;
            Assert.Equal(firstJob.ObsoleteInterval, _globalObsoleteInterval);
            Assert.Equal(firstJob.Delay, TimeSpan.Zero);
            Assert.NotNull(firstJob.NextJob);

            var secondJob = firstJob.NextJob;
            Assert.Equal(secondJob.ObsoleteInterval, _globalObsoleteInterval);
            Assert.Equal(secondJob.Delay, secondJobDelay);
            Assert.NotNull(secondJob.NextJob);

            var thirdJob = secondJob.NextJob;
            Assert.Equal(thirdJob.ObsoleteInterval, _globalObsoleteInterval);
            Assert.Equal(thirdJob.Delay, thirdJobDelay);
            Assert.Null(thirdJob.NextJob);

            _jobsAdderMock.Verify(a => a.AddEnqueueJob(It.IsAny<JobMetadata>()), Times.Once);
            _jobsAdderMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task Schedule_SetFailedStrategy_SaveInJobMetadata()
        {
            // Arrange
            var scheduledJob = new JobMetadata();
            const int maxRepeatCount = 5;

            var builder =
                new ParameterizedJobBuilder<TestJob, string>(_jobsAdderMock.Object, "HALLO", _globalObsoleteInterval)
                    .AddRepeatStrategy<DefaultRepeatStrategy>()
                    .MaxRepeatCount(maxRepeatCount);

            _jobsAdderMock.Setup(a => a.AddEnqueueJob(It.IsAny<JobMetadata>()))
                .Returns(Task.CompletedTask)
                .Callback((JobMetadata job) => scheduledJob = job);

            // Act
            await builder.Schedule();

            // Assert
            Assert.Equal(typeof(DefaultRepeatStrategy), scheduledJob.RepeatStrategy);
            Assert.Equal(scheduledJob.MaxRepeatCount, maxRepeatCount);
            _jobsAdderMock.Verify(a => a.AddEnqueueJob(It.IsAny<JobMetadata>()), Times.Once);
            _jobsAdderMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task MaxRepeatCountIsZero_ThrowException()
        {
            // Arrange
            const int maxRepeatCount = 0;

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ParameterizedJobBuilder<TestJob, string>(_jobsAdderMock.Object, "HALLO", _globalObsoleteInterval)
                    .MaxRepeatCount(maxRepeatCount));

        }
    }
}