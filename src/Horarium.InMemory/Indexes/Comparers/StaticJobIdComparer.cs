using System;
using Horarium.Repository;

namespace Horarium.InMemory.Indexes.Comparers
{
    public static class StaticJobIdComparer
    {
        public static int Compare(JobDb x, JobDb y)
        {
            return string.Compare(x.JobId, y.JobId, StringComparison.Ordinal);
        }
    }
}