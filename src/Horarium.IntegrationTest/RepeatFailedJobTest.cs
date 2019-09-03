using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Horarium.IntegrationTest.Jobs;
using Horarium.Interfaces;
using Xunit;

namespace Horarium.IntegrationTest
{
    [Collection(IntegrationTestCollection)]
    public class RepeatFailedJobTest : IntegrationTestBase
    {
        [Fact]
        public async Task RepeatFailedJob_UseRepeatStrategy()
        {
            var horarium = CreateHorariumServer();
            
            await horarium.Create<RepeatFailedJob, string>(string.Empty)
                .AddRepeatStrategy<RepeatFailedJobTestStrategy>()
                .MaxRepeatCount(5)
                .Schedule();

            var watch = Stopwatch.StartNew();
            
            while (!RepeatFailedJob.RepeatIsOk)
            {
                await Task.Delay(100);

                if (watch.Elapsed >= TimeSpan.FromSeconds(10))
                {
                    throw new Exception("Time is over");
                }
            }
            
            var executingTimes = RepeatFailedJob.ExecutingTime.ToArray();

            Assert.Equal(5, executingTimes.Length);

            var nextJobTime = executingTimes.First();

            foreach (var time in executingTimes)
            {
                Assert.Equal(nextJobTime, time, TimeSpan.FromMilliseconds(999));
                nextJobTime = time.AddSeconds(1);
            }
            
        }
    }
    
    public class RepeatFailedJobTestStrategy:  IFailedRepeatStrategy
    {
        public TimeSpan GetNextStartInterval(int countStarted)
        {
            return TimeSpan.FromSeconds(1);
        }
    }
}