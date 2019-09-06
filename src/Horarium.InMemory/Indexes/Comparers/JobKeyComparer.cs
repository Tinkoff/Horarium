using System;
using System.Collections.Generic;
using Horarium.Repository;

namespace Horarium.InMemory.Indexes.Comparers
{
    internal class JobKeyComparer : IComparer<JobDb>
    {
        public int Compare(JobDb x, JobDb y)
        {
            var result = string.Compare(x.JobKey, y.JobKey, StringComparison.Ordinal);
            
            return result == 0 ? StaticJobIdComparer.Compare(x, y) : result;
        }
    }
}