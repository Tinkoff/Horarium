using System.Collections.Generic;
using System.Threading.Tasks;

namespace Horarium.Interfaces
{
    public interface IStatisticsJobs
    {
        Task<Dictionary<JobStatus, int>> GetJobStatistic();
    }
}