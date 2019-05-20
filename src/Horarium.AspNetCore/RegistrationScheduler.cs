using System;
using Horarium.Interfaces;
using Horarium.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Horarium.AspNetCore
{
    public static class RegistrationHorariumExtension
    {
        public static IServiceCollection RegistrationHorarium(this IServiceCollection service,
            IJobRepository jobRepository)
        {
            return service.RegistrationHorarium(jobRepository, serviceProvider => new HorariumSettings());
        }

        public static IServiceCollection RegistrationHorarium(this IServiceCollection service,
            IJobRepository jobRepository,
            Func<IServiceProvider, HorariumSettings> func)
        {
            service.AddSingleton<IHorarium>(serviceProvider =>
            {
                var settings = func(serviceProvider);

                if (settings.JobFactory is DefaultJobFactory)
                {
                    settings.JobFactory = new JobFactory(serviceProvider);
                }

                if (settings.Logger is EmptyLogger)
                {
                    settings.Logger = new HorariumLogger(serviceProvider.GetService<ILogger<HorariumLogger>>());
                }

                return new HorariumServer(jobRepository, settings);
            });

            return service;
        }
    }
}