using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Horarium.Builders.JobSequenceBuilder;
using Horarium.Builders.Recurrent;
using Horarium.Builders.Parameterized;

namespace Horarium.Interfaces
{
    public interface IHorarium : IDisposable
    {
        /// <summary>
        /// Return count of jobs in status
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<JobStatus, int>> GetJobStatistic();

        /// <summary>
        /// Create one time job
        /// </summary>
        /// <typeparam name="TJob">Type of job, job will create from factory</typeparam>
        /// <typeparam name="TJobParam">Type of parameters</typeparam>
        /// <returns></returns>
        [Obsolete("use Schedule method instead")]
        IParameterizedJobBuilder Create<TJob, TJobParam>(TJobParam param) where TJob : IJob<TJobParam>;

        /// <summary>
        /// Create builder for recurrent job with cron 
        /// </summary>
        /// <param name="cron">Cron</param>
        /// <typeparam name="TJob">Type of job, job will create from factory</typeparam>
        /// <returns></returns>
        IRecurrentJobBuilder CreateRecurrent<TJob>(string cron) where TJob : IJobRecurrent;

        /// <summary>
        /// Create one time job
        /// </summary>
        /// <typeparam name="TJob">Type of job, job will create from factory</typeparam>
        /// <typeparam name="TJobParam">Type of parameters</typeparam>
        /// <returns></returns>
        Task Schedule<TJob, TJobParam>(TJobParam param, Action<IJobSequenceBuilder> configure = null) where TJob : IJob<TJobParam>;
    }
}