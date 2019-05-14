using System.Threading.Tasks;

namespace Horarium.Interfaces
{
    public interface IExecutorJob
    {
        Task Execute(JobMetadata jobMetadata);
    }
}