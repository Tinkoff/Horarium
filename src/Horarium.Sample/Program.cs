using System;
using System.Threading;
using System.Threading.Tasks;
using Horarium.Mongo;

namespace Horarium.Sample
{
    static class Program
    {
        static async Task Main()
        {
            await Task.Run(StartScheduler);
            Console.WriteLine("Start");
            Thread.Sleep(1000000);
            Console.ReadKey();
        }

        static async Task StartScheduler()
        {
            var horarium =
                new HorariumServer(MongoRepositoryFactory.Create("mongodb://localhost:27017/schedOpenSource"));

            await horarium.CreateRecurrent<TestRecurrentJob>(Cron.SecondInterval(10)).Schedule();

            await new HorariumClient(MongoRepositoryFactory.Create("mongodb://localhost:27017/schedOpenSource"))
                .GetJobStatistic();

            var firstJobDelay = TimeSpan.FromSeconds(20);

            var secondJobDelay = TimeSpan.FromSeconds(15);

            await horarium
                .Schedule<TestJob, int>(1, conf => conf // 1-st job
                                                   .WithDelay(firstJobDelay)
                                                   .Next<TestJob, int>(2) // 2-nd job
                                                   .WithDelay(secondJobDelay)
                                                   .Next<TestJob, int>(3) // 3-rd job (global obsolete from settings and no delay will be applied)
                                                   .Next<FailedTestJob, int>(4) // 4-th job failed with exception
                                                   .AddRepeatStrategy<CustomRepeatStrategy>()
                                                   .MaxRepeatCount(3)
                                                   .AddFallbackConfiguration(
                                                       x => x.GoToNextJob()) // execution continues after all attempts
                                                   .Next<FailedTestJob, int>(5) // 5-th job job failed with exception
                                                   .MaxRepeatCount(1)
                                                   .AddFallbackConfiguration(
                                                       x => x.ScheduleFallbackJob<FallbackTestJob, int>(6, builder =>
                                                       {
                                                           builder.Next<TestJob, int>(7);
                                                       })) // 6-th and 7-th jobs executes after all retries 
                );

            horarium.Start();
        }
    }
}