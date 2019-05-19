using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Horarium.Interfaces;
using Horarium.Repository;

namespace Horarium.Handlers
{
    public class RecurrentJobSettingsAdder : IRecurrentJobSettingsAdder
    {
        private readonly IJobRepository _jobRepository;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public RecurrentJobSettingsAdder(IJobRepository jobRepository, JsonSerializerSettings jsonSerializerSettings)
        {
            _jobRepository = jobRepository;
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public async Task Add(string cron, Type jobType, string jobKey)
        {
            var settings = new RecurrentJobSettingsMetadata(jobKey, jobType, cron);

            await _jobRepository.AddRecurrentJobSettings(RecurrentJobSettings.CreatedRecurrentJobSettings(settings));
        }
    }
}
