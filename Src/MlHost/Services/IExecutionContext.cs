using MlHostApi.Types;
using System.Threading;

namespace MlHost.Services
{
    public interface IExecutionContext
    {
        CancellationTokenSource TokenSource { get; }

        ExecutionState State { get; set; }

        ModelId? ModelId { get; set; }

        public bool ForceDeployment { get; set; }
    }
}