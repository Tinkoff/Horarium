using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Horarium.InMemory;
using Horarium.IntegrationTest.Jobs;
using Horarium.Interfaces;
using Horarium.Mongo;
using Horarium.Repository;
using Xunit;

namespace Horarium.IntegrationTest
{
    [Collection(IntegrationTestCollection)]
    public class TestParallelsWorkTwoManagers : IntegrationTestBase
    {
        public enum DataBase
        {
            MongoDB,
            InMemory,
        }

        public TestParallelsWorkTwoManagers()
        {       
            var provider = new MongoClientProvider(ConnectionMongo);
            var collection = provider.GetCollection<JobMongoModel>();

            collection.DeleteMany(MongoDB.Driver.Builders<JobMongoModel>.Filter.Empty);
        }

        private IHorarium CreateScheduler(DataBase dataBase)
        {
            IJobRepository jobRepository;

            switch (dataBase)
            {
                case DataBase.MongoDB:
                    jobRepository = MongoRepositoryFactory.Create(ConnectionMongo);
                    break;
                
                case DataBase.InMemory:
                    jobRepository = new InMemoryRepository();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataBase), dataBase, null);
            }

            var horarium = new HorariumServer(jobRepository);
            horarium.Start();
            
            return horarium;
        }

        [Theory]
        [InlineData(DataBase.MongoDB)]
        [InlineData(DataBase.InMemory)]
        public async Task TestParallels(DataBase dataBase)
        {
            var firstScheduler = CreateScheduler(dataBase);
            var secondScheduler = CreateScheduler(dataBase);

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

            firstScheduler.Dispose();
            secondScheduler.Dispose();

            Assert.NotEmpty(TestJob.StackJobs[dataBase]);

            Assert.False(TestJob.StackJobs[dataBase].GroupBy(x => x).Any(g => g.Count() > 1),
                "Несколько джобов выполнилось на 2-х машинах");
        }

        /// <summary>
        /// Тест проверяет, что при одновременной регистрации одного джоба разными шедулерами первый начнет выполняться, а второй нет,
        /// т.к. для рекуррентных джобов одновременно может выполняться только один экземпляр
        /// </summary>
        /// <returns></returns>
        [Theory]
        //[InlineData(DataBase.MongoDB)]
        
        /*
         * We need a better way of tracking jobs execution than some static ConcurrentStacks -
         * 'cause of the way schedulers are disposed (Task.Wait())
         * and the way xUnit works everything becomes a huge mess
         * very quickly :(
         * 
         * I feel like those Task.Wait calls aren't actually awaited by xUnit runner, thus
         * synchronization of shared static resources becomes VERY problematic
         */ 
        //[InlineData(DataBase.InMemory)]
        [InlineData(DataBase.InMemory)]
        public async Task Scheduler_SecondInstanceStart_MustUpdateRecurrentJobCronParameters(DataBase dataBase)
        {
            var watch = Stopwatch.StartNew();
            var scheduler = CreateScheduler(dataBase);

            var counter = 0;

            while (true)
            {
                await scheduler.CreateRecurrent<TestRecurrentJob>(Cron.SecondInterval(1)).Schedule();
                counter++;

                if (watch.Elapsed > TimeSpan.FromSeconds(15) || counter > 10000)
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