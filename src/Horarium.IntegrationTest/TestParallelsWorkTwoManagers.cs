using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Horarium.InMemory;
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
                await firstScheduler.Create<TestJob, TestJobParam>(new TestJobParam
                {
                    Counter = i,
                    DbType = dataBase
                }).Schedule();
                await Task.Delay(10);
            }
            
            await Task.Delay(10000);
            await Task.Delay(10000);

            firstScheduler.Dispose();
            firstScheduler.Dispose();
            secondScheduler.Dispose();

            Assert.NotEmpty(TestJob.StackJobs[dataBase]);

            Assert.False(TestJob.StackJobs[dataBase].GroupBy(x => x).Any(g => g.Count() > 1),
                "Несколько джобов выполнилось на 2-х машинах");
        }
        
    }
}