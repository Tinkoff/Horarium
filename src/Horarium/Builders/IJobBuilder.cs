using System;
using System.Threading.Tasks;

namespace Horarium.Builders
{
    [Obsolete("use IJobSequenceBuilder instead")]
    public interface IJobBuilder
    {
        /// <summary>
        /// Run current job
        /// </summary>
        /// <returns></returns>
        Task Schedule();
    }
}