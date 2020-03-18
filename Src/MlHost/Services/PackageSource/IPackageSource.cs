using System.IO;
using System.Threading.Tasks;

namespace MlHost.Services.PackageSource
{
    internal interface IPackageSource
    {
        Task<Stream> GetStream();
    }
}