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

namespace MlHost.Services
{
    internal class PackageSourceFromStorage : IPackageSource
    {
        private readonly IOption _option;
        private readonly ILogger<PackageSourceFromStorage> _logger;
        private readonly IExecutionContext _executionContext;
        private readonly IModelRepository _modelRepository;
        private readonly string _zipFilePath;

        public PackageSourceFromStorage(IOption option, ILogger<PackageSourceFromStorage> logger, IExecutionContext executionContext, IModelRepository modelRepository)
        {
            _option = option;
            _logger = logger;
            _executionContext = executionContext;
            _modelRepository = modelRepository;

            _zipFilePath = Path.Combine(_option.Deployment.PackageFolder, "ml-package.zip");
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
            sw.Stop();

            _logger.LogInformation($"Package file {_zipFilePath} has been downloaded from storage, {sw.ElapsedMilliseconds}ms");
            return true;
        }

        private async Task<bool> IsPackageCurrent()
        {
            BlobInfo? blobInfo = await _modelRepository.GetBlobInfo(_executionContext.ModelId!, _executionContext.TokenSource.Token);
            if (blobInfo == null) throw new InvalidOperationException($"Blob for model id {_executionContext.ModelId} does not exist");

            if (!File.Exists(_zipFilePath)) return false;

            using Stream fileStream = new FileStream(_zipFilePath, FileMode.Open);
            byte[] fileHash = MD5.Create().ComputeHash(fileStream);

            bool isCurrent = Enumerable.SequenceEqual(blobInfo.ContentHash, fileHash);
            _logger.LogInformation($"Package file '{_zipFilePath}' is {(isCurrent ? "current" : "not current will be updated")}");

            return isCurrent;
        }
    }
}
