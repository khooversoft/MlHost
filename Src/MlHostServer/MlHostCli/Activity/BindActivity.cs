using Microsoft.Extensions.Logging;
using MlHostSdk.Repository;
using MlHostSdk.Types;
using MlHostCli.Application;
using MlHostCli.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostCli.Activity
{
    internal class BindActivity
    {
        private const string _embeddedFileName = "RunModel.mlPackage";
        private const string _folderName = "MlPackage";
        private readonly IOption _option;
        private readonly IModelStore _modelRepository;
        private readonly ILogger<BindActivity> _logger;

        public BindActivity(IOption option, IModelStore modelRepository, ILogger<BindActivity> logger)
        {
            _option = option;
            _modelRepository = modelRepository;
            _logger = logger;
        }

        public async Task Bind(CancellationToken token)
        {
            _option.VsProject.VerifyAssert(x => File.Exists(x), $"Vs Project file={_option.VsProject} does not exist");
            _logger.LogInformation($"Binding model to ML Host {Path.GetFileNameWithoutExtension(_option.VsProject)}");

            await Download(token);

            _logger.LogInformation($"Update VS Project file {_option.VsProject} with embedded command for model.");
            new VsProject(_option.VsProject!)
                .Read()
                .Add($"{_folderName}\\{_embeddedFileName}")
                .Write();
        }

        private async Task Download(CancellationToken token)
        {
            var modelId = new ModelId(_option.ModelName!, _option.VersionId!);

            string filePath = Path.Combine(Path.GetDirectoryName(_option.VsProject)!, _folderName, _embeddedFileName);
            _logger.LogInformation($"Downloading model {modelId} to {filePath}");

            await _modelRepository.Download(modelId, filePath, token);
        }
    }
}
