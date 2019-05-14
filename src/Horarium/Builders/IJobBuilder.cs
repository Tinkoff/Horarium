using System.Threading.Tasks;

namespace Horarium.Builders
{
    public interface IJobBuilder
    {
        /// <summary>
        /// Run current job
        /// </summary>
        /// <returns></returns>
        Task Schedule();
    }
}