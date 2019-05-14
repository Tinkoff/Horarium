using System.Threading.Tasks;

namespace Horarium.Interfaces
{
    public interface IRunnerJobs
    {
        void Start();
        Task Stop();

    }
}