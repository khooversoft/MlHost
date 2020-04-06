using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHostApi.Repository;
using MlHostApi.Tools;
using MlHostApi.Types;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Toolbox.Repository;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHost.Services
{
    internal class PackageSourceFromStorage : IPackageSource
    {
        private readonly IOption _option;
        private readonly ILogger<PackageSourceFromStorage> _logger;
        private readonly IExecutionContext _executionContext;
        private readonly IModelRepository _modelRepository;
        private readonly IJson _json;
        private readonly string _zipFilePath;
        private readonly string _EtagPath;

        public PackageSourceFromStorage(IOption option, ILogger<PackageSourceFromStorage> logger, IExecutionContext executionContext, IModelRepository modelRepository, IJson json)
        {
            _option = option;
            _logger = logger;
            _executionContext = executionContext;
            _modelRepository = modelRepository;
            _json = json;
            _zipFilePath = Path.Combine(_option.Deployment.PackageFolder, "ml-package.zip");
            _EtagPath = Path.Combine(_option.Deployment.PackageFolder, "ml-package-etag.json");
        }

        public Task<Stream> GetStream()
        {
            return Task.FromResult<Stream>(new FileStream(_zipFilePath, FileMode.Open));
        }

        public async Task<bool> GetPackageIfRequired(bool overwrite)
        {
            Directory.CreateDirectory(_option.Deployment!.PackageFolder);

            if (!overwrite && (await IsPackageCurrent()))
            {
                _logger.LogInformation($"Package file {_zipFilePath} exist, no download is required");
                return true;
            }

            _logger.LogInformation($"Package file {_zipFilePath} does not exist, downloading from storage");

            var sw = Stopwatch.StartNew();
            await _modelRepository.Download(_executionContext.ModelId!, _zipFilePath, _executionContext.TokenSource.Token);
            await StoreETag();
            sw.Stop();

            _logger.LogInformation($"Package file {_zipFilePath} has been downloaded from storage, {sw.ElapsedMilliseconds}ms");
            return true;
        }

        private async Task<bool> IsPackageCurrent()
        {
            DatalakePathProperties pathProperties = await _modelRepository.GetPathProperties(_executionContext.ModelId!, _executionContext.TokenSource.Token);
            if (pathProperties == null) throw new InvalidOperationException($"Blob for model id {_executionContext.ModelId} does not exist");

            if (!File.Exists(_zipFilePath)) return false;
            if (!File.Exists(_EtagPath)) return false;

            string? etag = await ReadETag();
            return etag == pathProperties.ETag;
        }

        private async Task StoreETag()
        {
            DatalakePathProperties pathProperties = await _modelRepository.GetPathProperties(_executionContext.ModelId!, _executionContext.TokenSource.Token);
            pathProperties.ETag.VerifyNotEmpty(nameof(pathProperties.ETag));

            var store = new ETagStore
            {
                ETag = pathProperties.ETag!,
            };

            await store.WriteToFile(_EtagPath, _json);
        }

        private async Task<string?> ReadETag()
        {
            ETagStore? store = await FileTools.ReadFromFile<ETagStore>(_EtagPath, _json);
            return store?.ETag;
        }

        private class ETagStore
        {
            public string? ETag { get; set; }
        }
    }
}
