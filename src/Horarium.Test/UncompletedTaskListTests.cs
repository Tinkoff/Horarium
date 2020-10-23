using System;
using System.Threading.Tasks;
using Horarium.Handlers;
using Xunit;

namespace Horarium.Test
{
    public class UncompletedTaskListTests
    {
        private readonly UncompletedTaskList _uncompletedTaskList = new UncompletedTaskList();

        [Fact]
        public async Task Add_TaskWithAnyResult_KeepsTaskUntilCompleted()
        {
            var tcs1 = new TaskCompletionSource<bool>();
            var tcs2 = new TaskCompletionSource<bool>();
            var tcs3 = new TaskCompletionSource<bool>();

            _uncompletedTaskList.Add(tcs1.Task);
            _uncompletedTaskList.Add(tcs2.Task);
            _uncompletedTaskList.Add(tcs3.Task);

            Assert.Equal(3, _uncompletedTaskList.Count);

            tcs1.SetResult(false);
            await Task.Delay(TimeSpan.FromSeconds(1)); // give a chance to finish continuations
            Assert.Equal(2, _uncompletedTaskList.Count);

            tcs2.SetException(new ApplicationException());
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.Equal(1, _uncompletedTaskList.Count);

            tcs3.SetCanceled();
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.Equal(0, _uncompletedTaskList.Count);
        }

        [Fact]
        public async Task WhenAllCompleted_NoTasks_ReturnsCompletedTask()
        {
            // Act
            var whenAll = _uncompletedTaskList.WhenAllCompleted();

            // Assert
            Assert.True(whenAll.IsCompletedSuccessfully);
            await whenAll;
        }

        [Fact]
        public async Task WhenAllCompleted_TaskNotCompleted_AwaitsUntilTaskCompleted()
        {
            // Arrange
            var tcs = new TaskCompletionSource<bool>();
            _uncompletedTaskList.Add(tcs.Task);

            // Act
            var whenAll = _uncompletedTaskList.WhenAllCompleted();

            // Assert
            await Task.Delay(TimeSpan.FromSeconds(1)); // give a chance to finish any running tasks
            Assert.False(whenAll.IsCompleted);

            tcs.SetResult(false);
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.True(whenAll.IsCompletedSuccessfully);

            await whenAll;
        }

        [Fact]
        public async Task WhenAllCompleted_TaskFaulted_DoesNotThrow()
        {
            // Arrange
            _uncompletedTaskList.Add(Task.FromException(new ApplicationException()));

            // Act
            var whenAll = _uncompletedTaskList.WhenAllCompleted();

            await whenAll;
        }
    }
}