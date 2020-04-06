using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Services
{
    internal interface IExecutePython
    {
        Task Run(CancellationToken token);

        Task<bool> KillAnyRunningProcesses();
    }
}