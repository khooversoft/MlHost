using Microsoft.Extensions.Logging;
using MlHostSdk.Repository;
using MlHostSdk.Types;
using MlHostCli.Application;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;

namespace MlHostCli.Activity
{
    internal class DownloadModelActivity
    {
        private readonly IOption _option;
        private readonly IModelStore _modelRepository;
        private readonly ILogger<DownloadModelActivity> _logger;

        public DownloadModelActivity(IOption option, IModelStore modelRepository, ILogger<DownloadModelActivity> logger)
        {
            _option = option;
            _modelRepository = modelRepository;
            _logger = logger;
        }

        public Task Download(CancellationToken token)
        {
            var modelId = new ModelId(_option.ModelName!, _option.VersionId!);

            _logger.LogInformation($"Downloading model {modelId}");
            return _modelRepository.Download(modelId, _option.PackageFile!, token);
        }
    }
}
