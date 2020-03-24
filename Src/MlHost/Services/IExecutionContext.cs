using System.Threading;

namespace MlHost.Services
{
    public interface IExecutionContext
    {
        CancellationTokenSource TokenSource { get; }

        ExecutionState State { get; set; }
    }
}