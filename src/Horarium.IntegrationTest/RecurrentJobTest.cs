using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Horarium.IntegrationTest.Jobs;
using Xunit;

namespace Horarium.IntegrationTest
{
    [Collection(IntegrationTestCollection)]
    public class RecurrentJobTest : IntegrationTestBase
    {

        [Fact]
        public async Task RecurrentJob_RunEverySeconds()
        {
            var horarium = CreateHorariumServer();
            
            await horarium.CreateRecurrent<RecurrentJob>(Cron.Secondly()).Schedule();

            await Task.Delay(10000);

            horarium.Dispose();

            var executingTimes = RecurrentJob.ExecutingTime.ToArray();
            
            Assert.NotEmpty(executingTimes);
            
            var nextJobTime = executingTimes.First();

            foreach (var time in executingTimes)
            {
                Assert.Equal(nextJobTime, time, TimeSpan.FromMilliseconds(999));
                nextJobTime = time.AddSeconds(1);
            }
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
                await scheduler.CreateRecurrent<RecurrentJobForUpdate>(Cron.SecondInterval(1)).Schedule();

                if (watch.Elapsed > TimeSpan.FromSeconds(15))
                {
                    break;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5));

            scheduler.Dispose();

            Assert.Single(RecurrentJobForUpdate.StackJobs);
        }
    }
}