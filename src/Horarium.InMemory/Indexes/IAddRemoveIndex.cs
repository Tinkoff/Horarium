using Horarium.Repository;

namespace Horarium.InMemory.Indexes
{
    public interface IAddRemoveIndex
    {
        void Add(JobDb job);

        void Remove(JobDb job);

        int Count();
    }
}