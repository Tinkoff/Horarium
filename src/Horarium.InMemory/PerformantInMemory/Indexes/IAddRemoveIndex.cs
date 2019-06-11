using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory.Indexes
{
    public interface IAddRemoveIndex
    {
        void Add(JobDb job);

        void Remove(JobDb job);

        int Count();
    }
}