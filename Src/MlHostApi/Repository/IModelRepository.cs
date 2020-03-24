using MlHostApi.Models;
using MlHostApi.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MlHostApi.Repository
{
    public interface IModelRepository
    {
        Task Upload(string modelVersionFile, ModelId modelId, bool force, CancellationToken token);

        Task Download(ModelId modelId, string toFile, CancellationToken token);

        Task Delete(ModelId modelId, CancellationToken token);

        Task<bool> Exist(ModelId modelId, CancellationToken token);

        Task<HostConfigurationModel> ReadConfiguration(CancellationToken token);

        Task WriteConfiguration(HostConfigurationModel hostConfigurationModel, CancellationToken token);

        Task<IReadOnlyList<string>> Search(string prefix, string pattern, CancellationToken token);

        Task AddActivation(string hostName, ModelId modelId, CancellationToken token);

        Task RemoveActivation(string hostName, CancellationToken token);
    }
}