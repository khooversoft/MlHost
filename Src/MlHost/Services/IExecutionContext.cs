using System.Threading;

namespace MlHost.Services
{
    public interface IExecutionContext
    {
        CancellationTokenSource TokenSource { get; }

        bool Running { get; set; }
    }
}