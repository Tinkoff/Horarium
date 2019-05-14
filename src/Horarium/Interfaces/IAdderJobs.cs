using System.Threading.Tasks;

namespace Horarium.Interfaces
{
    public interface IAdderJobs
    {
        Task AddEnqueueJob(JobMetadata jobMetadata);

        Task AddRecurrentJob(JobMetadata jobMetadata);
    }
}