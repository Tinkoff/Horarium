using System;
using Horarium.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Horarium.AspNetCore
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object CreateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }

        public IDisposable BeginScope()
        {
            return _serviceProvider.CreateScope();
        }
    }
}