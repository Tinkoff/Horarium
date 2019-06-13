using System.Collections.Generic;
using Horarium.Repository;

namespace Horarium.InMemory.Indexes.Comparers
{
    public class JobIdComparer : IComparer<JobDb>
    {
        public int Compare(JobDb x, JobDb y)
        {
            return StaticJobIdComparer.Compare(x, y);
        }
    }
}