using System;
using System.Runtime.CompilerServices;
using Horarium.Handlers;
using Horarium.Interfaces;
using Horarium.Repository;

[assembly: InternalsVisibleTo("Horarium.Test")]

namespace Horarium
{
    public class HorariumServer : HorariumClient, IHorarium
    {
        private readonly HorariumSettings _settings;
        private IRunnerJobs _runnerJobs;
        private readonly IAdderJobs _adderJobs;

        private readonly IJobRepository _jobRepository;

        private readonly TimeSpan _timeoutStop = TimeSpan.FromSeconds(5);

        public HorariumServer(string mongoConnectionString)
            : this(new HorariumSettings(),
                MongoRepositoryFactory.Create(mongoConnectionString))
        {
        }
        
        public HorariumServer(string mongoConnectionString, HorariumSettings settings)
            : this(settings, MongoRepositoryFactory.Create(mongoConnectionString))
        {
        }

        private HorariumServer(HorariumSettings settings, IJobRepository jobRepository)
            : base(settings, jobRepository)
        {
            _settings = settings;
            _adderJobs = new AdderJobs(jobRepository, _settings.JsonSerializerSettings);
            _jobRepository = jobRepository;
            Start();
        }

        private void Start()
        {
            var executorJob = new ExecutorJob(_settings.JobFactory, _settings.Logger, _jobRepository, _adderJobs,
                _settings.JsonSerializerSettings);

            _runnerJobs = new RunnerJobs(_jobRepository, _settings, _settings.JsonSerializerSettings, _settings.Logger,
                executorJob);
            _runnerJobs.Start();
        }

        public new void Dispose()
        {
            _runnerJobs.Stop().Wait(_timeoutStop);
        }
    }
}