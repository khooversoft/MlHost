using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Services
{
    public interface IPackageDeployment
    {
        Task<string> Deploy(CancellationToken token);
    }
}