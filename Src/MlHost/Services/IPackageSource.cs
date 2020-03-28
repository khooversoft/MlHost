using MlHostApi.Repository;
using MlHostApi.Types;
using System.IO;
using System.Threading.Tasks;

namespace MlHost.Services
{
    internal interface IPackageSource
    {
        Task<bool> GetPackageIfRequired(bool overwrite);

        Task<Stream> GetStream();
    }
}