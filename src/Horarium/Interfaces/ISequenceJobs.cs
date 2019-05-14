using System;
using System.Threading.Tasks;

namespace Horarium.Interfaces
{
    public interface ISequenceJobs
    {
        /// <summary>
        /// Указываем следующий джоб для выполнения последовательности
        /// </summary>
        /// <param name="param">Параметры данного джоба, будут переданы при старте</param>
        /// <typeparam name="TJob">Тип джоба, сам джоб будет создан через фабрику</typeparam>
        /// <typeparam name="TJobParam">Тип параметров</typeparam>
        /// <returns></returns>
        ISequenceJobs NextJob<TJob, TJobParam>(TJobParam param) where TJob : IJob<TJobParam>;

        /// <summary>
        /// Указываем следующий джоб для выполнения последовательности с задержкой выполнения
        /// </summary>
        /// <param name="param">Параметры данного джоба, будут переданы при старте</param>
        /// <param name="delay">Задержка выполнения джоба, после окончания выполнения предыдущего</param>
        /// <typeparam name="TJob">Тип джоба, сам джоб будет создан через фабрику</typeparam>
        /// <typeparam name="TJobParam">Тип параметров</typeparam>
        /// <returns></returns>
        ISequenceJobs NextJob<TJob, TJobParam>(TJobParam param, TimeSpan delay) where TJob : IJob<TJobParam>;

        /// <summary>
        /// Запускает данную последовательность 
        /// </summary>
        /// <returns></returns>
        Task Run();
    }
}