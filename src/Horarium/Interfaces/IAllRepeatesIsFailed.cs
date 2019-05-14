using System;
using System.Threading.Tasks;

namespace Horarium.Interfaces
{
    public interface IAllRepeatesIsFailed
    {
        Task FailedEvent(object param, Exception ex);
    }
}
