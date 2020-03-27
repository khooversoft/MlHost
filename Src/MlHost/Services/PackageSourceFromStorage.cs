using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHostApi.Repository;
using MlHostApi.Types;
using System;
using System.Diagnostics;
using System.IO;
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

        public async Task<bool> GetPackageIfRequired(bool overwrite)
        {
            Directory.CreateDirectory(_option.Deployment!.PackageFolder);

            if (File.Exists(_zipFilePath) && !overwrite)
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

        public async Task<Stream> GetStream()
        {
            await GetPackageIfRequired(false);

            return new FileStream(_zipFilePath, FileMode.Open);
        }
    }
}
