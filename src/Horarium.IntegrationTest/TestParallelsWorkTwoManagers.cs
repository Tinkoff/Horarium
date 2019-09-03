using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Horarium.IntegrationTest.Jobs;
using Xunit;

namespace Horarium.IntegrationTest
{
    [Collection(IntegrationTestCollection)]
    public class TestParallelsWorkTwoManagers : IntegrationTestBase
    {
        [Fact]
        public async Task TestParallels()
        {
            var firstScheduler = CreateHorariumServer();
            var secondScheduler = CreateHorariumServer();

            for (var i = 0; i < 1000; i++)
            {
                await firstScheduler.Create<TestJob, int>(i).Schedule();
                await Task.Delay(10);
            }

            await Task.Delay(10000);

            firstScheduler.Dispose();
            secondScheduler.Dispose();

            Assert.NotEmpty(TestJob.StackJobs);

            Assert.False(TestJob.StackJobs.GroupBy(x => x).Any(g => g.Count() > 1),
                "Same job was executed multiple times");
        }

        
    }
}