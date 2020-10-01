using MlHostSdk.Types;
using System.Threading;

namespace MlHost.Models
{
    public interface IExecutionContext
    {
        CancellationTokenSource TokenSource { get; }

        ExecutionState State { get; set; }

        string? DeploymentFolder { get; set; }
    }
}