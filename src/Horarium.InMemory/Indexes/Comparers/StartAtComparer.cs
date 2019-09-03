using System.Collections.Generic;
using Horarium.Repository;

namespace Horarium.InMemory.Indexes.Comparers
{
    internal class StartAtComparer : IComparer<JobDb>
    {
        public int Compare(JobDb x, JobDb y)
        {
            if (x.StartAt < y.StartAt) return -1;
            if (x.StartAt > y.StartAt) return 1;

            return StaticJobIdComparer.Compare(x, y);
        }
    }
}