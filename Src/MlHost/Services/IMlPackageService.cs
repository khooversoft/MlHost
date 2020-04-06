using System.Threading.Tasks;

namespace MlHost.Services
{
    internal interface IMlPackageService
    {
        Task<bool> Start();
        Task Stop();
    }
}