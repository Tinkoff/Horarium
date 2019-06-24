using System;

namespace Horarium.Interfaces
{
    public interface IJobScope : IDisposable
    {
        object CreateJob(Type type);
    }
}