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

        /// <summary>
        /// Тест проверяет, что при одновременной регистрации одного джоба разными шедулерами первый начнет выполняться, а второй нет,
        /// т.к. для рекуррентных джобов одновременно может выполняться только один экземпляр
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Scheduler_SecondInstanceStart_MustUpdateRecurrentJobCronParameters()
        {
            var watch = Stopwatch.StartNew();
            var scheduler = CreateHorariumServer();

            while (true)
            {
                await scheduler.CreateRecurrent<TestRecurrentJob>(Cron.SecondInterval(1)).Schedule();

                if (watch.Elapsed > TimeSpan.FromSeconds(15))
                {
                    break;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5));

            scheduler.Dispose();

            Assert.Single(TestRecurrentJob.StackJobs);
        }
    }
}