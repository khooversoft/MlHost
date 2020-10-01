using MlHostSdk.Models;
using System.Threading.Tasks;

namespace MlHost.Services
{
    public interface IPredictService
    {
        Task<PredictResponse> Submit(Question request);
    }
}
