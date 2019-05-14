using System;
using Horarium.Interfaces;

namespace Horarium
{
    public class DefaultJobFactory : IJobFactory
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