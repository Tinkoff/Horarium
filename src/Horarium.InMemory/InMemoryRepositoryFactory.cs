using System;
using Horarium.Repository;

namespace Horarium.InMemory
{
    public static class InMemoryRepositoryFactory
    {
        private static readonly Lazy<InMemoryRepository> 
            Repository = new Lazy<InMemoryRepository>(() => new InMemoryRepository());

        public static IJobRepository Create() => Repository.Value;
    }
}