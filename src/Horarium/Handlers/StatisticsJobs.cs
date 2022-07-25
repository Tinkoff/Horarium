using System.Collections.Generic;
using System.Threading.Tasks;
using Horarium.Interfaces;
using Horarium.Repository;

namespace Horarium.Handlers
{
    public class StatisticsJobs : IStatisticsJobs
    {
        private readonly IJobRepository _jobRepository;

        public StatisticsJobs(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public Task<Dictionary<JobStatus, int>> GetJobStatistic()
        {
            return _jobRepository.GetJobStatistic();
        }
    }
}