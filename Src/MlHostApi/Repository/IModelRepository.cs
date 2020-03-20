using MlHostApi.Models;
using MlHostApi.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MlHostApi.Repository
{
    public interface IModelRepository
    {
        Task Delete(ModelId modelId, CancellationToken token);

        Task Download(ModelId modelId, string toFile, CancellationToken token);

        Task<HostConfigurationModel> ReadConfiguration(CancellationToken token);

        Task<IReadOnlyList<string>> Search(string prefix, string pattern, CancellationToken token);

        Task Upload(string modelVersionFile, ModelId modelId, CancellationToken token);

        Task WriteConfiguration(HostConfigurationModel hostConfigurationModel, CancellationToken token);
    }
}