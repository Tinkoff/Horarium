using System;

namespace Horarium.Interfaces
{
    public interface IJobFactory
    {
        object CreateJob(Type type);
        IDisposable BeginScope();
    }
}