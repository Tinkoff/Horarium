using System.Collections.Generic;
using Horarium.Repository;

namespace Horarium.InMemory.Indexes.Comparers
{
    internal class StartedExecutingComparer : IComparer<JobDb>
    {
        public int Compare(JobDb x, JobDb y)
        {
            if (x.StartedExecuting < y.StartedExecuting) return -1;
            if (x.StartedExecuting > y.StartedExecuting) return 1;

            return StaticJobIdComparer.Compare(x, y);
        }
    }
}