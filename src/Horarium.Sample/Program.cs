using System;
using System.Threading;
using System.Threading.Tasks;
using Horarium.Mongo;

namespace Horarium.Sample
{
    static class Program
    {

        static void Main()
        {
            Task.Run(StartScheduler);
            Console.WriteLine("Start");
            Thread.Sleep(1000000);
            Console.ReadKey();
        }

        static async Task StartScheduler()
        {
             var horarium = new HorariumServer(MongoRepositoryFactory.Create("mongodb://localhost:27017/schedOpenSource"));
            
            
            await horarium.CreateRecurrent<TestRecurrentJob>(Cron.SecondInterval(10)).Schedule();

            await new HorariumClient(MongoRepositoryFactory.Create("mongodb://localhost:27017/schedOpenSource"))
                .GetJobStatistic();

            var firstJobDelay = TimeSpan.FromSeconds(20);
            
            var secondJobDelay = TimeSpan.FromSeconds(15);
        
            await horarium
                .Create<TestJob, int>(1) // 1-st job
                    .WithDelay(firstJobDelay)
                .Next<TestJob, int>(2) // 2-nd job
                    .WithDelay(secondJobDelay)
                .Next<TestJob, int>(3) // 3-rd job (global obsolete from settings and no delay will be applied)
                .Schedule();

            await horarium.Create<TestJob, int>(666)
                .WithDelay(TimeSpan.FromSeconds(25))
                .Schedule();
            
            
            await Task.Delay(20000);

            await horarium.CreateRecurrent<TestRecurrentJob>(Cron.SecondInterval(15))
                .WithKey(nameof(TestRecurrentJob))
                .Schedule();
        }
    }
}