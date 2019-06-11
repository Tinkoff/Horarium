using System;
using Horarium.Repository;

namespace Horarium.InMemory.PerformantInMemory.Indexes.Comparers
{
    public static class FailoverComparer
    {
        public static int Compare(JobDb x, JobDb y)
        {
            return string.Compare(x.JobId, y.JobId, StringComparison.Ordinal);
        }
    }
}