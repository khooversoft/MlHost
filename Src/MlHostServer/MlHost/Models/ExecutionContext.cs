using MlHostSdk.Types;
using System.Threading;

namespace MlHost.Models
{
    internal class ExecutionContext : IExecutionContext
    {
        public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

        public ExecutionState State { get; set; }

        public string? DeploymentFolder { get; set; }
    }
}
