using System.Threading.Tasks;

namespace MlHost.Services
{
    internal interface IExecutePython
    {
        Task Run();

        void KillAnyRunningProcesses();
    }
}