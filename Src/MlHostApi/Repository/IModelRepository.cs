using MlHostApi.Models;
using MlHostApi.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Repository;

namespace MlHostApi.Repository
{
    public interface IModelRepository
    {
        Task Upload(string modelVersionFile, ModelId modelId, bool force, CancellationToken token);

        Task Download(ModelId modelId, string toFile, CancellationToken token);

        Task Delete(ModelId modelId, CancellationToken token);

        Task<bool> Exist(ModelId modelId, CancellationToken token);

        Task<DatalakePathProperties> GetPathProperties(ModelId modelId, CancellationToken token);

        Task<HostConfigurationModel> ReadConfiguration(CancellationToken token);

        Task WriteConfiguration(HostConfigurationModel hostConfigurationModel, CancellationToken token);

        Task<IReadOnlyList<DatalakePathItem>> Search(string? prefix, string pattern, CancellationToken token);

        Task AddActivation(string hostName, ModelId modelId, CancellationToken token);

        Task RemoveActivation(string hostName, CancellationToken token);

        Task<ModelId?> GetRegistration(string hostName, CancellationToken token);

        Task<T?> Read<T>(string path, CancellationToken token) where T : class;

        Task Write<T>(string path, T value, CancellationToken token) where T : class;
    }
}