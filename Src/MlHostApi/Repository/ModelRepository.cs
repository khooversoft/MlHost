using MlHostApi.Models;
using MlHostApi.Services;
using MlHostApi.Tools;
using MlHostApi.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MlHostApi.Repository
{
    /// <summary>
    /// Model repository uses Azure blob storage to store, manage, and configure the ML hosts
    /// 
    /// Only paths will be store so that the blob storage can be moved to another subscription, storage account, other storage system, etc...
    /// 
    /// The storage has the following URI schema.
    /// 
    /// "ml-models/{modelName}/{versionId}"
    /// "host-configuration.json"
    /// 
    /// </summary>
    public class ModelRepository : IModelRepository
    {
        private const string _hostConfiguratonPath = "host-configuration.json";

        private readonly IBlobRepository _blobRepository;
        private readonly IJson _json;

        public ModelRepository(IBlobRepository blobRepository, IJson json)
        {
            _blobRepository = blobRepository;
            _json = json;
        }

        public async Task Upload(string modelVersionFile, ModelId modelId, bool force, CancellationToken token)
        {
            modelVersionFile.VerifyNotEmpty(nameof(modelVersionFile));
            modelId.VerifyNotNull(nameof(modelId));

            using Stream fileStream = new FileStream(modelVersionFile, FileMode.Open);
            await _blobRepository.Upload(fileStream, modelId.ToBlobPath(), force, token);
        }

        public async Task Download(ModelId modelId, string toFile, CancellationToken token)
        {
            modelId.VerifyNotNull(nameof(modelId));
            toFile.VerifyNotEmpty(nameof(toFile));

            using Stream fileStream = new FileStream(toFile, FileMode.Create);
            await _blobRepository.Download(modelId.ToBlobPath(), fileStream, token);
        }

        public async Task Delete(ModelId modelId, CancellationToken token)
        {
            modelId.VerifyNotNull(nameof(modelId));

            await RemoveActivation(modelId, token);
            await _blobRepository.Delete(modelId.ToBlobPath(), token);
        }

        public async Task<bool> Exist(ModelId modelId, CancellationToken token)
        {
            modelId.VerifyNotNull(nameof(modelId));
            IReadOnlyList<string> list = await Search(modelId.ToBlobPath(), modelId.ToRegexPattern(), token);
            return list.Count == 1;
        }

        public Task<BlobInfo?> GetBlobInfo(ModelId modelId, CancellationToken token)
        {
            return _blobRepository.GetBlobInfo(modelId.ToBlobPath(), token);
        }

        public async Task<HostConfigurationModel> ReadConfiguration(CancellationToken token)
        {
            bool exist = await _blobRepository.Exist(_hostConfiguratonPath, token);
            if (!exist) return new HostConfigurationModel();

            byte[] data = await _blobRepository.Read(_hostConfiguratonPath, token);
            string json = Encoding.UTF8.GetString(data);

            return _json.Deserialize<HostConfigurationModel>(json).VerifyNotNull("Deserialize failed");
        }

        public async Task WriteConfiguration(HostConfigurationModel hostConfigurationModel, CancellationToken token)
        {
            hostConfigurationModel.VerifyNotNull(nameof(hostConfigurationModel));

            string json = _json.Serialize(hostConfigurationModel);
            byte[] data = Encoding.UTF8.GetBytes(json);

            await _blobRepository.Write(_hostConfiguratonPath, data, token);
        }

        public Task<IReadOnlyList<string>> Search(string prefix, string pattern, CancellationToken token)
        {
            if (pattern == "*" || pattern.ToNullIfEmpty() == null)
            {
                return _blobRepository.Search(prefix, x => true, token);
            }

            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            return _blobRepository.Search(prefix, x => regex.Match(x).Success, token);
        }

        public async Task AddActivation(string hostName, ModelId modelId, CancellationToken token)
        {
            hostName.VerifyNotEmpty(nameof(hostName));
            modelId.VerifyNotNull(nameof(modelId));

            IDictionary<string, ModelId> hostAssigments = (await ReadConfiguration(token)).ToDictionary();

            if (hostAssigments.TryGetValue(hostName, out ModelId? readModelId) && modelId == readModelId) return;

            await hostAssigments
                .Action(x => x[hostName] = modelId)
                .ToModel()
                .Func(async x => await WriteConfiguration(x, token));
        }

        public async Task RemoveActivation(string hostName, CancellationToken token)
        {
            hostName.VerifyNotEmpty(nameof(hostName));

            IDictionary<string, ModelId> hostAssigments = (await ReadConfiguration(token)).ToDictionary();

            if (!hostAssigments.Remove(hostName)) return;

            await hostAssigments
                .ToModel()
                .Func(async x => await WriteConfiguration(x, token));
        }

        public async Task<ModelId?> GetRegistration(string hostName, CancellationToken token)
        {
            IDictionary<string, ModelId> hostAssigments = (await ReadConfiguration(token)).ToDictionary();

            if (hostAssigments.TryGetValue(hostName, out ModelId? value)) return value;
            return null;
        }
    }
}
