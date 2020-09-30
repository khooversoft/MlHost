using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Services
{
    internal interface IExecutePython
    {
        Task Start();

        void Stop();
    }
}