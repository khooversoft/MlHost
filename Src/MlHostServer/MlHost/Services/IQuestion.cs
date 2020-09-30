using MlHostSdk.Models;
using System.Threading.Tasks;

namespace MlHost.Services
{
    public interface IQuestion
    {
        Task<PredictResponse> Ask(Question request);
    }
}
