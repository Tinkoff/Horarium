using System;
using System.Threading.Tasks;

namespace Horarium.Interfaces
{
    public interface ISequenceJobs
    {
        /// <summary>
        /// Indicate the next job for sequence execution
        /// </summary>
        /// <param name="param">The job parameters, will be send at start</param>
        /// <typeparam name="TJob">Job type, the job will be created through the factory</typeparam>
        /// <typeparam name="TJobParam">Parameters type</typeparam>
        /// <returns></returns>
        ISequenceJobs NextJob<TJob, TJobParam>(TJobParam param) where TJob : IJob<TJobParam>;

        /// <summary>
        /// Indicate the next job for sequence with delay execution
        /// </summary>
        /// <param name="param">The job parameters, will be send at start</param>
        /// <param name="delay">Job execution delay, after the previous execution</param>
        /// <typeparam name="TJob">Job type, the job will be created through the factory</typeparam>
        /// <typeparam name="TJobParam">Parameters type</typeparam>
        /// <returns></returns>
        ISequenceJobs NextJob<TJob, TJobParam>(TJobParam param, TimeSpan delay) where TJob : IJob<TJobParam>;

        /// <summary>
        /// Run this sequence
        /// </summary>
        /// <returns></returns>
        Task Run();
    }
}