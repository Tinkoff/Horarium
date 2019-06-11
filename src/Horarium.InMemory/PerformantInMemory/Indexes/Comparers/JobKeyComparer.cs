using System;
using System.Collections.Generic;
using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory.Indexes.Comparers
{
    public class JobKeyComparer : IComparer<JobDb>
    {
        public int Compare(JobDb x, JobDb y)
        {
            var result = string.Compare(x.JobKey, y.JobKey, StringComparison.Ordinal);
            
            return result == 0 ? FailoverComparer.Compare(x, y) : result;
        }
    }
}