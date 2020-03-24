using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHostApi.Repository;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MlHost.Services
{
    internal class PackageSourceFromStorage : IPackageSource
    {
        private readonly IOption _option;
        private readonly ILogger<PackageSourceFromStorage> _logger;
        private readonly IBlobRepository _blobRepository;
        private readonly IExecutionContext _executionContext;

        public PackageSourceFromStorage(IOption option, ILogger<PackageSourceFromStorage> logger, IBlobRepository blobRepository, IExecutionContext executionContext)
        {
            _option = option;
            _logger = logger;
            _blobRepository = blobRepository;
            _executionContext = executionContext;
        }

        public async Task<Stream> GetStream()
        {
            string zipFilePath = await GetZipFileIfRequired();

            return new FileStream(zipFilePath, FileMode.Open);
        }

        private async Task<string> GetZipFileIfRequired()
        {

            Directory.CreateDirectory(_option.Deployment.PackageFolder);

            string zipFilePath = Path.Combine(_option.Deployment.PackageFolder, "ml-package.zip");
            if (File.Exists(zipFilePath) && !_option.ForceDeployment)
            {
                _logger.LogInformation($"Zip file {zipFilePath} exist, no download is required");
                return zipFilePath;
            }

            _logger.LogInformation($"Zip file {zipFilePath} does not exist, downloading from storage");

            var sw = Stopwatch.StartNew();

            using Stream toStream = new FileStream(zipFilePath, FileMode.Create);
            await _blobRepository.Download(_option.ZipFileUri, toStream, _executionContext.TokenSource.Token);

            sw.Stop();

            _logger.LogInformation($"Zip file {zipFilePath} has been downloaded from storage, {sw.ElapsedMilliseconds}ms");

            return zipFilePath;
        }
    }
}
