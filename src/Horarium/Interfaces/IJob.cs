using System.Threading.Tasks;

namespace Horarium.Interfaces
{
    public interface IJob<in TJobParam>
    {
        Task Execute(TJobParam param);
    }
    
    public interface IJobRecurrent
    {
        Task Execute();
    }
}