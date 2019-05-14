using System.Threading.Tasks;
using Horarium.Interfaces;
using Horarium.Repository;
using Newtonsoft.Json;

namespace Horarium.Handlers
{
    public class AdderJobs : IAdderJobs
    {
        private readonly IJobRepository _jobRepository;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly IRecurrentJobSettingsAdder _recurrentJobSettingsAdder;

        public AdderJobs(IJobRepository jobRepository, JsonSerializerSettings jsonSerializerSettings)
        {
            _jobRepository = jobRepository;
            _jsonSerializerSettings = jsonSerializerSettings;
            _recurrentJobSettingsAdder = new RecurrentJobSettingsAdder(_jobRepository, _jsonSerializerSettings);
        }

        public Task AddEnqueueJob(JobMetadata jobMetadata)
        {
            var job = JobDb.CreatedJobDb(jobMetadata, _jsonSerializerSettings);

            return _jobRepository.AddJob(job);
        }


        public async Task AddRecurrentJob(JobMetadata jobMetadata)
        {
            await _recurrentJobSettingsAdder.Add(jobMetadata.Cron, jobMetadata.JobType, jobMetadata.JobKey);

            await _jobRepository.AddRecurrentJob(JobDb.CreatedJobDb(jobMetadata, _jsonSerializerSettings));
        }
    }
}