using System.Threading.Tasks;
using Horarium.IntegrationTest.Jobs;
using Xunit;

namespace Horarium.IntegrationTest
{
    [Collection(IntegrationTestCollection)]
    public class OneTimeJobTest: IntegrationTestBase
    {
        [Fact]
        public async Task OneTimeJob_RunAfterAdded()
        {
            var horarium = CreateHorariumServer();
            
            horarium.Start();

            await horarium.Create<OneTimeJob, int>(5).Schedule();
            
            await Task.Delay(1000);

            horarium.Dispose();

            Assert.True(OneTimeJob.Run);
        }
    }
}