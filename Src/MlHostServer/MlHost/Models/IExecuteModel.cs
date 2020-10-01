using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Models
{
    internal interface IExecuteModel
    {
        Task Start();

        void Stop();
    }
}