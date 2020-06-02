using MlHostApi.Types;
using System.Threading;

namespace MlHost.Services
{
    internal class ExecutionContext : IExecutionContext
    {
        public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

        public ExecutionState State { get; set; }

        public string? DeploymentFolder { get; set; }
    }
}
