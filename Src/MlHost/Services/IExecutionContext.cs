using MlHostApi.Types;
using System.Threading;

namespace MlHost.Services
{
    public interface IExecutionContext
    {
        CancellationTokenSource TokenSource { get; }

        ExecutionState State { get; set; }

        string? DeploymentFolder { get; set; }
    }
}