using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest.Jobs
{
    public class TestJob : IJob<TestJobParam>
    {
        public static readonly Dictionary<TestParallelsWorkTwoManagers.DataBase, ConcurrentStack<int>> StackJobs =
            new Dictionary<TestParallelsWorkTwoManagers.DataBase, ConcurrentStack<int>>
            {
                {TestParallelsWorkTwoManagers.DataBase.InMemory, new ConcurrentStack<int>()},
                {TestParallelsWorkTwoManagers.DataBase.MongoDB, new ConcurrentStack<int>()}
            };

        public async Task Execute(TestJobParam param)
        {
            StackJobs[param.DbType].Push(param.Counter);
            await Task.Delay(30);
        }
    }

    public class TestJobParam
    {
        public int Counter { get; set; }

        public TestParallelsWorkTwoManagers.DataBase DbType { get; set; }
    }
}