using MlHostApi.Repository;
using MlHostApi.Services;
using MlHostApi.Tools;
using MlHostApi.Types;
using MlHostCli.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MlHostCli.Activity
{
    internal class DownloadModelActivity
    {
        private readonly IOption _option;
        private readonly IModelRepository _modelRepository;
        private readonly ITelemetry _telemetry;

        public DownloadModelActivity(IOption option, IModelRepository modelRepository, ITelemetry telemetry)
        {
            _option = option;
            _modelRepository = modelRepository;
            _telemetry = telemetry;
        }

        public Task Download(CancellationToken token)
        {
            var modelId = new ModelId(_option.ModelName!, _option.VersionId!);

            _telemetry.WriteLine($"Downloading model {modelId}");
            return _modelRepository.Download(modelId, _option.PackageFile!, token);
        }
    }
}
