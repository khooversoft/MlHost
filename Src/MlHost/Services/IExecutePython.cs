using System.Threading.Tasks;

namespace MlHost.Services
{
    internal interface IExecutePython
    {
        Task Run();

        Task<bool> KillAnyRunningProcesses();
    }
}