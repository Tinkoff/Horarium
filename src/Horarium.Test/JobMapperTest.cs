using Newtonsoft.Json;
using Horarium.Repository;
using Xunit;

namespace Horarium.Test
{
    public class JobMapperTest
    {
        private string _strJobType = "Horarium.Test.TestJob, Horarium.Test";
        private string _strJobParamType = "System.String, System.Private.CoreLib";
        private string _strJobParam = @"""test""";

        [Fact]
        public void ToJobDb()
        {
            var job = new JobMetadata()
            {
                JobType = typeof(TestJob),
                JobParam = "test"
            };

            var jobDb = JobDb.CreatedJobDb(job,new JsonSerializerSettings());

            Assert.Equal(jobDb.JobType, _strJobType);
            Assert.Equal(jobDb.JobParamType, _strJobParamType);
            Assert.Equal(jobDb.JobParam, _strJobParam);
        }

        [Fact]
        public void ToJob()
        {

            var job = new JobDb()
            {
                JobType = _strJobType,
                JobParamType = _strJobParamType,
                JobParam = _strJobParam
            };

            var jobDb = job.ToJob(new JsonSerializerSettings());

            Assert.Equal(typeof(TestJob), jobDb.JobType);
            Assert.Equal("test", jobDb.JobParam );
        }
    }
}