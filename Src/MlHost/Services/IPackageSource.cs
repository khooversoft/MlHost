using System.IO;
using System.Threading.Tasks;

namespace MlHost.Services
{
    internal interface IPackageSource
    {
        Task<Stream> GetStream();
    }
}