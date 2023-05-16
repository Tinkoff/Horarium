using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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

        private readonly IJobRepository _jobRepository;

        public HorariumServer(IJobRepository jobRepository)
            : this(jobRepository, new HorariumSettings())
        {
        }

        public HorariumServer(IJobRepository jobRepository, HorariumSettings settings)
            : base(jobRepository, settings)
        {
            _settings = settings;
            _jobRepository = jobRepository;
        }

        public void Start()
        {
            var executorJob = new ExecutorJob(_jobRepository, _settings);

            _runnerJobs = new RunnerJobs(_jobRepository, _settings, _settings.JsonSerializerSettings, _settings.Logger,
                executorJob, new UncompletedTaskList());
            _runnerJobs.Start();
        }

        public Task Stop(CancellationToken stopCancellationToken)
        {
            return _runnerJobs.Stop(stopCancellationToken);
        }

        public new void Dispose()
        {
            Stop(CancellationToken.None);
        }
    }
}