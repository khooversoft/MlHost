using MlHostSdk.Models;
using MlHostSdk.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Repository;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostSdk.Repository
{
    /// <summary>
    /// Model repository uses Azure blob storage to store, manage, and configure the ML hosts
    /// 
    /// Only paths will be store so that the blob storage can be moved to another subscription, storage account, other storage system, etc...
    /// 
    /// The storage has the following URI schema.
    /// 
    /// "ml-models/{modelName}/{versionId}"
    /// 
    /// </summary>
    public class DatalakeModelStore : IModelStore
    {
        private readonly IDatalakeStore _datalakeRepository;
        private readonly IJson _json;

        public DatalakeModelStore(IDatalakeStore datalakeRepository, IJson json)
        {
            _datalakeRepository = datalakeRepository;
            _json = json;
        }

        public async Task Upload(string modelVersionFile, ModelId modelId, bool force, CancellationToken token)
        {
            modelVersionFile.VerifyNotEmpty(nameof(modelVersionFile));
            modelId.VerifyNotNull(nameof(modelId));

            using Stream fileStream = new FileStream(modelVersionFile, FileMode.Open);
            await _datalakeRepository.Upload(fileStream, modelId.ToPath(), force, token);
        }

        public async Task Download(ModelId modelId, string toFile, CancellationToken token)
        {
            modelId.VerifyNotNull(nameof(modelId));
            toFile.VerifyNotEmpty(nameof(toFile));

            Directory.CreateDirectory(Path.GetDirectoryName(toFile));
            using Stream fileStream = new FileStream(toFile, FileMode.Create);
            await _datalakeRepository.Download(modelId.ToPath(), fileStream, token);
        }

        public async Task Delete(ModelId modelId, CancellationToken token)
        {
            modelId.VerifyNotNull(nameof(modelId));

            await _datalakeRepository.Delete(modelId.ToPath(), token);
        }

        public async Task<bool> Exist(ModelId modelId, CancellationToken token)
        {
            modelId.VerifyNotNull(nameof(modelId));
            return await _datalakeRepository.Exist(modelId.ToPath(), token);
        }

        public Task<DatalakePathProperties> GetPathProperties(ModelId modelId, CancellationToken token)
        {
            modelId.VerifyNotNull(nameof(modelId));
            return _datalakeRepository.GetPathProperties(modelId.ToPath(), token);
        }

        public Task<IReadOnlyList<DatalakePathItem>> Search(string? prefix, string pattern, CancellationToken token)
        {
            if (pattern == "*" || pattern.ToNullIfEmpty() == null)
            {
                return _datalakeRepository.Search(prefix, x => true, true, token);
            }

            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            return _datalakeRepository.Search(prefix, x => regex.Match(x.Name).Success, true, token);
        }

        public async Task<T?> Read<T>(string path, CancellationToken token) where T : class
        {
            path.VerifyNotNull(nameof(path));

            bool exist = await _datalakeRepository.Exist(path, token);
            if (!exist) return null;

            byte[] data = await _datalakeRepository.Read(path, token);
            string jsonString = Encoding.UTF8.GetString(data);

            return _json.Deserialize<T>(jsonString).VerifyNotNull("Deserialize failed");
        }

        public async Task Write<T>(string path, T value, CancellationToken token) where T : class
        {
            path.VerifyNotEmpty(nameof(path));
            value.VerifyNotNull(nameof(value));

            string jsonString = _json.Serialize(value);
            byte[] data = Encoding.UTF8.GetBytes(jsonString);

            await _datalakeRepository.Write(path, data, true, token);
        }
    }
}
