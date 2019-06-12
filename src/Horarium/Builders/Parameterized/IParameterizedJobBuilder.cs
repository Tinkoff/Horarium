using Horarium.Interfaces;

namespace Horarium.Builders.Parameterized
{
    public interface IParameterizedJobBuilder : IJobBuilder, IDelayedJobBuilder<IParameterizedJobBuilder>
    {
        /// <summary>
        /// Create next job, it run after previous job
        /// </summary>
        /// <param name="parameters"></param>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TJobParam"></typeparam>
        /// <returns></returns>
        IParameterizedJobBuilder Next<TJob, TJobParam>(TJobParam parameters) where TJob : IJob<TJobParam>;

        /// <summary>
        /// Add custom failed repeat strategy for job
        /// </summary>
        /// <typeparam name="TRepeat"></typeparam>
        /// <returns></returns>
        IParameterizedJobBuilder AddRepeatStrategy<TRepeat>() where TRepeat : IFailedRepeatStrategy;
        
        /// <summary>
        /// Set custom max failed repeat count
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        IParameterizedJobBuilder MaxRepeatCount(int count);
    }
}