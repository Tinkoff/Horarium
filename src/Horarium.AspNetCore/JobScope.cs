using System;
using Horarium.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Horarium.AspNetCore
{
    public class JobScope : IJobScope
    {
        private readonly IServiceScope _serviceScope;

        public JobScope(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
        }

        public object CreateJob(Type type)
        {
            return _serviceScope.ServiceProvider.GetService(type);
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}