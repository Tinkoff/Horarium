using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Horarium.Interfaces;
using Horarium.Repository;
using Newtonsoft.Json;
using Horarium.Builders.Recurrent;


namespace Horarium.Handlers
{
    public class ExecutorJob: IExecutorJob
    {
        private readonly IJobFactory _jobFactory;
        private readonly IHorariumLogger _horariumLogger;
        private readonly IJobRepository _jobRepository;
        private readonly IAdderJobs _adderJobs;
        private readonly int _maxCountRepeat = 10;
        private readonly int _increaseRepeat = 10;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public ExecutorJob(IJobFactory jobFactory, 
            IHorariumLogger horariumLogger, 
            IJobRepository jobRepository, 
            IAdderJobs adderJobs, 
            JsonSerializerSettings jsonSerializerSettings)
        {
            _jobFactory = jobFactory;
            _horariumLogger = horariumLogger;
            _jobRepository = jobRepository;
            _adderJobs = adderJobs;
            _jsonSerializerSettings = jsonSerializerSettings;
        }
        
        public Task Execute(JobMetadata jobMetadata)
        {
            if (jobMetadata.JobType.GetTypeInfo().GetInterfaces().Contains(typeof(IJobRecurrent)))
            {
                return ExecuteJobRecurrent(jobMetadata);
            }

            return ExecuteJob(jobMetadata);
        }

        private async Task ExecuteJob(JobMetadata jobMetadata)
        {
            dynamic jobImplementation = null;

            try
            {
                using (_jobFactory.BeginScope())
                {
                    try
                    {
                        jobImplementation = _jobFactory.CreateJob(jobMetadata.JobType);
                    }
                    catch (Exception ex)
                    {
                        //Дополнительное логирование ошибки, когда джоб не может быть создан
                        _horariumLogger.Error($"Ошибка создания джоба {jobMetadata.JobType}", ex);
                        throw;
                    }

                    _horariumLogger.Debug("got jobImplementation -" + jobImplementation.GetType());
                    _horariumLogger.Debug("got JobParam -" + jobMetadata.JobParam.GetType());
                    await jobImplementation.Execute((dynamic) jobMetadata.JobParam);

                    _horariumLogger.Debug("jobMetadata excecuted");

                    if (jobMetadata.NextJob != null)
                    {
                        jobMetadata.NextJob.StartAt = DateTime.Now + jobMetadata.NextJob.Delay.GetValueOrDefault();

                        await _jobRepository.AddJob(JobDb.CreatedJobDb(jobMetadata.NextJob, _jsonSerializerSettings));

                        _horariumLogger.Debug("next jobMetadata added");
                    }

                    await _jobRepository.RemoveJob(jobMetadata.JobId);

                    _horariumLogger.Debug("jobMetadata saved success");
                }
            }
            catch (Exception ex)
            {
                await HandleFailed(jobMetadata, ex, jobImplementation);
            }
        }

        private async Task ExecuteJobRecurrent(JobMetadata jobMetadata)
        {
            try
            {
                using (_jobFactory.BeginScope())
                {
                    dynamic jobImplementation;
                    try
                    {
                        jobImplementation = _jobFactory.CreateJob(jobMetadata.JobType);
                    }
                    catch (Exception ex)
                    {
                        //Дополнительное логирование ошибки, когда джоб не может быть создан
                        _horariumLogger.Error($"Ошибка создания джоба {jobMetadata.JobType}", ex);
                        throw;
                    }

                    _horariumLogger.Debug("got jobImplementation -" + jobImplementation.GetType());

                    await jobImplementation.Execute();

                    _horariumLogger.Debug("jobMetadata excecuted");

                    await _jobRepository.RemoveJob(jobMetadata.JobId);

                    _horariumLogger.Debug("jobMetadata saved success");
                }
            }
            catch (Exception ex)
            {
                await _jobRepository.FailedJob(jobMetadata.JobId, ex);
            }
            finally
            {
                await ScheduleRecurrentNextTime(jobMetadata);
            }
        }
        
        private async Task HandleFailed(JobMetadata jobMetadata, Exception ex, dynamic jobImplementation)
        {
            _horariumLogger.Debug(ex);
            if (jobMetadata.CountStarted >= _maxCountRepeat)
            {
                if (jobImplementation != null && jobImplementation is IAllRepeatesIsFailed)
                {
                    await jobImplementation.FailedEvent((dynamic) jobMetadata.JobParam, ex);
                }

                await _jobRepository.FailedJob(jobMetadata.JobId, ex);
                _horariumLogger.Debug("jobMetadata saved failed");
            }
            else
            {
                await _jobRepository.RepeatJob(jobMetadata.JobId, GetNextStartFailedJobTime(jobMetadata), ex);
                _horariumLogger.Debug("jobMetadata saved repeat");
            }
        }
        
        private DateTime GetNextStartFailedJobTime(JobMetadata jobMetadata)
        {
            var timeSpan = TimeSpan.FromMinutes(_increaseRepeat * jobMetadata.CountStarted);
            return DateTime.Now + timeSpan;
        }

        private async Task ScheduleRecurrentNextTime(JobMetadata metadata)
        {
            var cron = await _jobRepository.GetCronForRecurrentJob(metadata.JobKey);
            
            await new RecurrentJobBuilder(_adderJobs, cron, metadata.JobType, metadata.ObsoleteInterval)
                .WithKey(metadata.JobKey)
                .Schedule();
        }
    }
}