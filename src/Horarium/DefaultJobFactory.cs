using System;
using Horarium.Interfaces;

namespace Horarium
{
    public class DefaultJobScopeFactory : IJobScopeFactory
    {
        public IJobScope Create()
        {
            return new DefaultJobScope();
        }

        public class DefaultJobScope : IJobScope
        {
            public object CreateJob(Type type)
            {
                return Activator.CreateInstance(type);
            }

            public void Dispose()
            {
            }
        }
    }
}