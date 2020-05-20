using MlHostApi.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MlProcess.Services
{
    internal interface IHttpRest
    {
        Task<PredictResponse> Invoke(string uri, string question, CancellationToken token);
    }
}