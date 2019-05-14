using System;
using Horarium.Interfaces;

namespace Horarium.IntegrationTest
{
    public class JobFactory : IJobFactory
    {
        public object CreateJob(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public IDisposable BeginScope()
        {
            return new Disposable();
        }

        private class Disposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}