using System.Collections.Generic;
using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory.Indexes.Comparers
{
    public class JobIdComparer : IComparer<JobDb>
    {
        public int Compare(JobDb x, JobDb y)
        {
            return FailoverComparer.Compare(x, y);
        }
    }
}