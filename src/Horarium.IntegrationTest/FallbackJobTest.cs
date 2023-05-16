using System.Threading.Tasks;
using Horarium.IntegrationTest.Jobs.Fallback;
using Xunit;

namespace Horarium.IntegrationTest
{
    [Collection(IntegrationTestCollection)]
    public class FallbackJobTest : IntegrationTestBase
    {
        public FallbackJobTest()
        {
            FallbackNextJob.ExecutedCount = 0;
            FallbackMainJob.ExecutedCount = 0;
            FallbackJob.ExecutedCount = 0;
        }
        
        [Fact]
        public async Task FallbackJobAdded_FallbackJobExecuted()
        {
            var horarium = CreateHorariumServer();

            var mainJobRepeatCount = 2;
            await horarium.Schedule<FallbackMainJob, int>(1, conf => 
                                                              conf.MaxRepeatCount(mainJobRepeatCount)
                                                                  .AddRepeatStrategy<FallbackRepeatStrategy>()
                                                                  .AddFallbackConfiguration(configure =>
                                                                      configure
                                                                          .ScheduleFallbackJob<FallbackJob, int>(
                                                                              2,
                                                                              builder =>
                                                                              {
                                                                                  builder
                                                                                      .Next<FallbackNextJob,
                                                                                          int>(3);
                                                                              })));

            await Task.Delay(7000);

            horarium.Dispose();

            Assert.Equal(mainJobRepeatCount, FallbackMainJob.ExecutedCount);
            Assert.Equal(1, FallbackJob.ExecutedCount);
            Assert.Equal(1, FallbackNextJob.ExecutedCount);
        }
        
        [Fact]
        public async Task FallbackJobGoNextStrategy_NextJobExecuted()
        {
            var horarium = CreateHorariumServer();

            var mainJobRepeatCount = 2;
            await horarium.Schedule<FallbackMainJob, int>(1, conf => 
                                                              conf.MaxRepeatCount(mainJobRepeatCount)
                                                                  .AddRepeatStrategy<FallbackRepeatStrategy>()
                                                                  .AddFallbackConfiguration(
                                                                      configure => configure.GoToNextJob())
                                                                  .Next<FallbackNextJob, int>(2)
            );
            
            await Task.Delay(7000);

            horarium.Dispose();

            Assert.Equal(mainJobRepeatCount,  FallbackMainJob.ExecutedCount);
            Assert.Equal(1, FallbackNextJob.ExecutedCount);
        }
    }
}