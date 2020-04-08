using MlHostApi.Types;
using System.Threading;

namespace MlHost.Services
{
    internal class ExecutionContext : IExecutionContext
    {
        public ExecutionContext() { }

        public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

        public ExecutionState State { get; set; }

        public ModelId? ModelId { get; set; }
    }
}
