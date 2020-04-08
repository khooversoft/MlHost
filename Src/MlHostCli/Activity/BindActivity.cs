using MlHostApi.Repository;
using MlHostApi.Types;
using MlHostCli.Application;
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
        private readonly IOption _option;
        private readonly IModelRepository _modelRepository;
        private readonly ITelemetry _telemetry;

        public BindActivity(IOption option, IModelRepository modelRepository, ITelemetry telemetry)
        {
            _option = option;
            _modelRepository = modelRepository;
            _telemetry = telemetry;
        }

        public async Task Bind(CancellationToken token)
        {
            _telemetry.WriteLine($"Binding model to ML Host");

            string filePath = await Download(token);
            UnpackPackage(filePath, token);

            FileTools.DeleteDirectory(Path.GetDirectoryName(filePath)!);
        }

        private async Task<string> Download(CancellationToken token)
        {
            var modelId = new ModelId(_option.ModelName!, _option.VersionId!);

            string directory = Path.Combine(Path.GetTempPath(), nameof(MlHostCli));
            string filePath = new MlPackageFile(modelId, directory).FilePath;
            _telemetry.WriteLine($"Downloading model {modelId} to {filePath}");

            await _modelRepository.Download(modelId, filePath, token);
            return filePath;
        }

        private void UnpackPackage(string filePath, CancellationToken token)
        {
            _telemetry.WriteLine($"Unpacking {filePath} to install path {_option.InstallPath}");
            ZipArchiveTools.ExtractFromZipFile(filePath, _option.InstallPath!, token);
        }
    }
}
