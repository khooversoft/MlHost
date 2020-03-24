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

        public PackageSourceFromStorage(IOption option, ILogger<PackageSourceFromStorage> logger, IExecutionContext executionContext, IModelRepository modelRepository)
        {
            _option = option;
            _logger = logger;
            _executionContext = executionContext;
            _modelRepository = modelRepository;
        }

        public async Task<Stream> GetStream(ModelId modelId)
        {
            string zipFilePath = await GetZipFileIfRequired(modelId);

            return new FileStream(zipFilePath, FileMode.Open);
        }

        private async Task<string> GetZipFileIfRequired(ModelId modelId)
        {
            Directory.CreateDirectory(_option.Deployment!.PackageFolder);

            string zipFilePath = Path.Combine(_option.Deployment.PackageFolder, "ml-package.zip");
            if (File.Exists(zipFilePath) && !_option.ForceDeployment)
            {
                _logger.LogInformation($"Zip file {zipFilePath} exist, no download is required");
                return zipFilePath;
            }

            _logger.LogInformation($"Zip file {zipFilePath} does not exist, downloading from storage");

            var sw = Stopwatch.StartNew();
            await _modelRepository.Download(modelId, zipFilePath, _executionContext.TokenSource.Token);
            sw.Stop();

            _logger.LogInformation($"Zip file {zipFilePath} has been downloaded from storage, {sw.ElapsedMilliseconds}ms");
            return zipFilePath;
        }
    }
}
