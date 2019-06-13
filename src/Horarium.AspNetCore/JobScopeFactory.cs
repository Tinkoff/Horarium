using System;
using Horarium.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Horarium.AspNetCore
{
    public class JobScopeFactory : IJobScopeFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobScopeFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJobScope Create()
        {
            return new JobScope(_serviceProvider.CreateScope());
        }
    }
}