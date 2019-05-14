using System;
using System.Threading.Tasks;

namespace Horarium.Interfaces
{
    public interface IRecurrentJobSettingsAdder
    {
        Task Add(string cron, Type jobType, string jobKey);
    }
}
