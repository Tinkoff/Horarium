using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Horarium.Builders.Recurrent;
using Horarium.Handlers;
using Horarium.Interfaces;
using Horarium.Repository;
using Horarium.Builders.Parameterized;

namespace Horarium
{
    public class HorariumClient : IHorarium
    {
        private readonly HorariumSettings _settings;
        private readonly IAdderJobs _adderJobs;
        private readonly IStatisticsJobs _statisticsJobs;

        public HorariumClient(IJobRepository jobRepository):this(jobRepository, new HorariumSettings())
        {
        }
        

        public HorariumClient(IJobRepository jobRepository, HorariumSettings settings)
        {
            _settings = settings;
            _adderJobs = new AdderJobs(jobRepository, settings.JsonSerializerSettings);
            _statisticsJobs = new StatisticsJobs(jobRepository);
        }

        private TimeSpan GlobalObsoleteInterval => _settings.ObsoleteExecutingJob;

        public IRecurrentJobBuilder CreateRecurrent<TJob>(string cron) where TJob : IJobRecurrent
        {
            return new RecurrentJobBuilder(_adderJobs, cron, typeof(TJob), GlobalObsoleteInterval);
        }

        public IParameterizedJobBuilder Create<TJob, TJobParam>(TJobParam param) where TJob : IJob<TJobParam>
        {
            return new ParameterizedJobBuilder<TJob, TJobParam>(_adderJobs, param, _settings.ObsoleteExecutingJob);
        }

        public Task<Dictionary<JobStatus, int>> GetJobStatistic()
        {
            return _statisticsJobs.GetJobStatistic();
        }

        public void Dispose()
        {
        }
    }
}