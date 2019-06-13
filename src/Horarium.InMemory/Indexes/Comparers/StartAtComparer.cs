using System.Collections.Generic;
using Horarium.Repository;

namespace Horarium.InMemory.Indexes.Comparers
{
    public class StartAtComparer : IComparer<JobDb>
    {
        public int Compare(JobDb x, JobDb y)
        {
            if (x.StartAt < y.StartAt) return -1;
            if (x.StartAt > y.StartAt) return 1;

            return StaticJobIdComparer.Compare(x, y);
        }
    }
}