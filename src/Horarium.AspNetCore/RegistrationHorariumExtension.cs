using System;
using Horarium.Interfaces;
using Horarium.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Horarium.AspNetCore
{
    public static class RegistrationHorariumExtension
    {
        public static IServiceCollection AddHorariumServer(this IServiceCollection service,
            IJobRepository jobRepository)
        {
            return service.AddHorariumServer(jobRepository, serviceProvider => new HorariumSettings());
        }

        public static IServiceCollection AddHorariumServer(this IServiceCollection service,
            IJobRepository jobRepository,
            Func<IServiceProvider, HorariumSettings> func)
        {
            service.AddSingleton<IHorarium>(serviceProvider =>
            {
                var settings = func(serviceProvider);

                PrepareSettings(settings, serviceProvider);

                return new HorariumServer(jobRepository, settings);
            });

            return service;
        }
        
        public static IServiceCollection AddHorariumClient(this IServiceCollection service,
            IJobRepository jobRepository)
        {
            return service.AddHorariumClient(jobRepository, serviceProvider => new HorariumSettings());
        }

        public static IServiceCollection AddHorariumClient(this IServiceCollection service,
            IJobRepository jobRepository,
            Func<IServiceProvider, HorariumSettings> func)
        {
            service.AddSingleton<IHorarium>(serviceProvider =>
            {
                var settings = func(serviceProvider);

                PrepareSettings(settings, serviceProvider);

                return new HorariumClient(jobRepository, settings);
            });

            return service;
        }

        public static void StartHorariumServer(this IServiceProvider serviceProvider)
        {
            var server = (HorariumServer)serviceProvider.GetService<IHorarium>();
            server.Start();
        }

        private static void PrepareSettings(HorariumSettings settings, IServiceProvider serviceProvider)
        {
            if (settings.JobFactory is DefaultJobFactory)
            {
                settings.JobFactory = new JobFactory(serviceProvider);
            }

            if (settings.Logger is EmptyLogger)
            {
                settings.Logger = new HorariumLogger(serviceProvider.GetService<ILogger<HorariumLogger>>());
            }
        }
    }
}