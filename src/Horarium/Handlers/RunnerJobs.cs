using System;
using System.Threading;
using System.Threading.Tasks;
using Horarium.Interfaces;
using Horarium.Repository;
using Newtonsoft.Json;

namespace Horarium.Handlers
{
    public class RunnerJobs : IRunnerJobs
    {
        private readonly string _machineName = Environment.MachineName + "_" + Guid.NewGuid();
        private readonly IJobRepository _jobRepository;
        private readonly HorariumSettings _settings;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly IHorariumLogger _horariumLogger;
        private readonly IExecutorJob _executorJob;
        private Task _runnerTask;
        private readonly IUncompletedTaskList _uncompletedTaskList;
        
        private readonly TimeSpan _defaultJobThrottleInterval = TimeSpan.FromMilliseconds(100);

        private CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        public RunnerJobs(IJobRepository jobRepository,
            HorariumSettings settings,
            JsonSerializerSettings jsonSerializerSettings,
            IHorariumLogger horariumLogger, IExecutorJob executorJob,
            IUncompletedTaskList uncompletedTaskList)
        {
            _jobRepository = jobRepository;
            _settings = settings;
            _jsonSerializerSettings = jsonSerializerSettings;
            _horariumLogger = horariumLogger;
            _executorJob = executorJob;
            _uncompletedTaskList = uncompletedTaskList;
        }

        public void Start()
        {
            _horariumLogger.Debug("Starting RunnerJob...");

            _cancellationToken = _cancelTokenSource.Token;
            _runnerTask = Task.Run(StartRunner, _cancellationToken);

            _horariumLogger.Debug("Started RunnerJob...");
        }

        public async Task Stop(CancellationToken stopCancellationToken)
        {
            _cancelTokenSource.Cancel(false);

            try
            {
                await _runnerTask;
            }
            catch (TaskCanceledException)
            {
                //watcher был остановлен
            }

            await _uncompletedTaskList.WhenAllCompleted(stopCancellationToken);

            _horariumLogger.Debug("Stopped DeleterJob");
        }

        private async Task StartRunner()
        {
            try
            {
                await StartRunnerInternal(_cancellationToken);
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _horariumLogger.Error("Остановлен StartPeriodicCheckStates", ex);
                _runnerTask = StartRunner();
                throw;
            }
        }

        private async Task<JobMetadata> GetReadyJob()
        {
            try
            {
                var job = await _jobRepository.GetReadyJob(_machineName, _settings.ObsoleteExecutingJob);

                if (job != null)
                    return job.ToJob(_jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                _horariumLogger.Error("Ошибка получения джоба из базы", ex);
            }

            return null;
        }

        private async Task StartRunnerInternal(CancellationToken cancellationToken)
        {
            var jobWaitTime = _settings.IntervalStartJob;

            while (true)
            {
                var isJobRan = await TryRunJob(cancellationToken, jobWaitTime);
                if (!_settings.JobThrottleSettings.UseJobThrottle)
                {
                    jobWaitTime = _settings.IntervalStartJob;
                    continue;
                }

                jobWaitTime = !isJobRan ? GetNextIntervalStartJob(jobWaitTime) : _settings.IntervalStartJob;
            }
        }
        
        private async Task<bool> TryRunJob(CancellationToken cancellationToken, TimeSpan waitTime)
        {
            for (var i = 0; i < _settings.JobThrottleSettings.JobRetrievalAttempts; i++)
            {
                var job = await GetReadyJob();
                var isJobReady = job != null;

                if (isJobReady)
                {
                    _horariumLogger.Debug("Try to Run jobMetadata...");

                    var jobTask = Task.Run(() => _executorJob.Execute(job), CancellationToken.None);
                    _uncompletedTaskList.Add(jobTask);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                if (!waitTime.Equals(TimeSpan.Zero))
                {
                    await Task.Delay(waitTime, cancellationToken);
                }
                
                if (isJobReady)
                {
                    return true;
                }
            }

            return false;
        }

        private TimeSpan GetNextIntervalStartJob(TimeSpan currentInterval)
        {
            if (currentInterval.Equals(TimeSpan.Zero))
            {
                return _defaultJobThrottleInterval;
            }

            var nextInterval =
                currentInterval +
                TimeSpan.FromTicks((long) (currentInterval.Ticks * _settings.JobThrottleSettings.IntervalMultiplier));

            var maxInterval = _settings.JobThrottleSettings.MaxJobThrottleInterval;

            return nextInterval > maxInterval ? maxInterval : nextInterval;
        }
    }
}