using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Horarium.Handlers;
using Horarium.Interfaces;
using Horarium.Repository;
using Xunit;

namespace Horarium.Test
{
    public class RunnerJobTest
    {
        [Fact]
        public async Task Start_Stop()
        {
            // Arrange
            var jobRepositoryMock = new Mock<IJobRepository>();

            jobRepositoryMock.Setup(x => x.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()));

            var runnerJobs = new RunnerJobs(jobRepositoryMock.Object,
                new HorariumSettings(),
                new JsonSerializerSettings(),
                Mock.Of<IHorariumLogger>(),
                Mock.Of<IExecutorJob>(),
                Mock.Of<IUncompletedTaskList>());

            // Act
            runnerJobs.Start();

            await Task.Delay(TimeSpan.FromSeconds(1));

            await runnerJobs.Stop(CancellationToken.None);

            jobRepositoryMock.Invocations.Clear();

            await Task.Delay(TimeSpan.FromSeconds(1));

            // Assert
            jobRepositoryMock.Verify(x => x.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task Start_RecoverAfterIntervalTimeout_AfterFailedDB()
        {
            // Arrange
            var jobRepositoryMock = new Mock<IJobRepository>();

            var settings = new HorariumSettings
            {
                IntervalStartJob = TimeSpan.FromSeconds(2),
            };

            var runnerJobs = new RunnerJobs(jobRepositoryMock.Object,
                settings,
                new JsonSerializerSettings(),
                Mock.Of<IHorariumLogger>(),
                Mock.Of<IExecutorJob>(),
                Mock.Of<IUncompletedTaskList>());

            jobRepositoryMock.SetupSequence(x => x.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .ThrowsAsync(new Exception())
                .ReturnsAsync(new JobDb());

            // Act
            runnerJobs.Start();
            await Task.Delay(settings.IntervalStartJob + TimeSpan.FromMilliseconds(1000));

            // Assert
            jobRepositoryMock.Verify(r => r.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.AtLeast(2));
        }

        [Fact]
        public async Task Start_WontRecoverBeforeIntervalTimeout_AfterFailedDB()
        {
            // Arrange
            var jobRepositoryMock = new Mock<IJobRepository>();

            var settings = new HorariumSettings
            {
                IntervalStartJob = TimeSpan.FromSeconds(2),
            };

            var runnerJobs = new RunnerJobs(jobRepositoryMock.Object,
                settings,
                new JsonSerializerSettings(),
                Mock.Of<IHorariumLogger>(),
                Mock.Of<IExecutorJob>(),
                Mock.Of<IUncompletedTaskList>());

            jobRepositoryMock.SetupSequence(x => x.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .ThrowsAsync(new Exception())
                .ReturnsAsync(new JobDb());

            // Act
            runnerJobs.Start();
            await Task.Delay(settings.IntervalStartJob - TimeSpan.FromMilliseconds(500));

            // Assert
            jobRepositoryMock.Verify(r => r.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task Start_ExecutionWithDelay_WithThrottle()
        {
            // Arrange
            var jobRepositoryMock = new Mock<IJobRepository>();

            var settings = new HorariumSettings
            {
                IntervalStartJob = TimeSpan.FromSeconds(1),
                JobThrottleSettings = new JobThrottleSettings
                {
                    UseJobThrottle = true,
                    IntervalMultiplier = 1,
                    JobRetrievalAttempts = 1
                }
            };

            var runnerJobs = new RunnerJobs(jobRepositoryMock.Object,
                settings,
                new JsonSerializerSettings(),
                Mock.Of<IHorariumLogger>(),
                Mock.Of<IExecutorJob>(),
                Mock.Of<IUncompletedTaskList>());

            // Act
            runnerJobs.Start();
            await Task.Delay(settings.IntervalStartJob - TimeSpan.FromMilliseconds(500));
            jobRepositoryMock.Invocations.Clear();

            await Task.Delay(settings.IntervalStartJob + settings.IntervalStartJob.Multiply(settings.JobThrottleSettings.IntervalMultiplier));
            
            // Assert
            jobRepositoryMock.Verify(r => r.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
        }
        
        [Fact]
        public async Task Start_ExecutionWithDelay_IncreaseInterval()
        {
            // Arrange
            var jobRepositoryMock = new Mock<IJobRepository>();

            jobRepositoryMock.Setup(x => x.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(() => null);

            var settings = new HorariumSettings
            {
                IntervalStartJob = TimeSpan.FromSeconds(1),
                JobThrottleSettings = new JobThrottleSettings
                {
                    UseJobThrottle = true,
                    IntervalMultiplier = 1,
                    JobRetrievalAttempts = 1,
                }
            };

            var runnerJobs = new RunnerJobs(jobRepositoryMock.Object,
                settings,
                new JsonSerializerSettings(),
                Mock.Of<IHorariumLogger>(),
                Mock.Of<IExecutorJob>(),
                Mock.Of<IUncompletedTaskList>());

            // Act
            runnerJobs.Start();
            await Task.Delay(settings.IntervalStartJob - TimeSpan.FromMilliseconds(500));
            jobRepositoryMock.Invocations.Clear();

            var interval = settings.IntervalStartJob +
                           settings.IntervalStartJob.Multiply(settings.JobThrottleSettings.IntervalMultiplier);
            await Task.Delay(interval);
            interval += settings.IntervalStartJob.Multiply(settings.JobThrottleSettings.IntervalMultiplier);
            await Task.Delay(interval);
            interval += settings.IntervalStartJob.Multiply(settings.JobThrottleSettings.IntervalMultiplier);
            await Task.Delay(interval);

            // Assert
            jobRepositoryMock.Verify(r => r.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Exactly(3));
        }
        
        [Fact]
        public async Task Start_ExecutionWithDelay_MaxInterval()
        {
            // Arrange
            var jobRepositoryMock = new Mock<IJobRepository>();

            jobRepositoryMock.Setup(x => x.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(() => null);

            var settings = new HorariumSettings
            {
                IntervalStartJob = TimeSpan.FromSeconds(1),
                JobThrottleSettings = new JobThrottleSettings
                {
                    UseJobThrottle = true,
                    IntervalMultiplier = 1,
                    JobRetrievalAttempts = 1,
                    MaxJobThrottleInterval = TimeSpan.FromSeconds(1)
                }
            };

            var runnerJobs = new RunnerJobs(jobRepositoryMock.Object,
                settings,
                new JsonSerializerSettings(),
                Mock.Of<IHorariumLogger>(),
                Mock.Of<IExecutorJob>(),
                Mock.Of<IUncompletedTaskList>());

            // Act
            runnerJobs.Start();
            await Task.Delay(settings.IntervalStartJob - TimeSpan.FromMilliseconds(500));
            jobRepositoryMock.Invocations.Clear();

            await Task.Delay(TimeSpan.FromSeconds(5));
            // Assert
            jobRepositoryMock.Verify(r => r.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Exactly(5));
        }

        [Fact]
        public async Task Start_NextJobStarted_AddsJobTaskToUncompletedTasks()
        {
            // Arrange
            var jobRepositoryMock = new Mock<IJobRepository>();
            var uncompletedTaskList = new Mock<IUncompletedTaskList>();

            uncompletedTaskList.Setup(x => x.Add(It.IsAny<Task>()));

            jobRepositoryMock.Setup(x => x.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(new JobDb
                {
                    JobType = typeof(object).ToString(),
                });

            var runnerJobs = new RunnerJobs(jobRepositoryMock.Object,
                new HorariumSettings
                {
                    IntervalStartJob = TimeSpan.FromHours(1), // prevent second job from starting
                },
                new JsonSerializerSettings(),
                Mock.Of<IHorariumLogger>(),
                Mock.Of<IExecutorJob>(),
                uncompletedTaskList.Object);

            // Act
            runnerJobs.Start();
            await Task.Delay(TimeSpan.FromSeconds(5));
            await runnerJobs.Stop(CancellationToken.None);

            // Assert
            uncompletedTaskList.Verify(x=>x.Add(It.IsAny<Task>()), Times.Once);
        }

        [Fact]
        public async Task StopAsync_AwaitsWhenAllCompleted()
        {
            // Arrange
            var jobRepositoryMock = new Mock<IJobRepository>();
            var uncompletedTaskList = new Mock<IUncompletedTaskList>();
            var cancellationToken = new CancellationTokenSource().Token;

            var settings = new HorariumSettings
            {
                IntervalStartJob = TimeSpan.FromSeconds(2),
            };

            var runnerJobs = new RunnerJobs(jobRepositoryMock.Object,
                settings,
                new JsonSerializerSettings(),
                Mock.Of<IHorariumLogger>(),
                Mock.Of<IExecutorJob>(),
                uncompletedTaskList.Object);

            jobRepositoryMock.Setup(x => x.GetReadyJob(It.IsAny<string>(), It.IsAny<TimeSpan>()));

            // Act
            runnerJobs.Start();
            await Task.Delay(TimeSpan.FromSeconds(1));
            await runnerJobs.Stop(cancellationToken);

            // Assert
            uncompletedTaskList.Verify(x => x.WhenAllCompleted(cancellationToken), Times.Once);
        }
    }
}