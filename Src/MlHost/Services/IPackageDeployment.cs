using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Services
{
    internal interface IPackageDeployment
    {
        Task Deploy();
    }
}