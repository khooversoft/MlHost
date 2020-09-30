using Microsoft.Extensions.Logging;
using MlHostSdk.Repository;
using MlHostSdk.Types;
using MlHostCli.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostCli.Activity
{
    internal class UploadModelActivity
    {
        private readonly IOption _option;
        private readonly IModelStore _modelRepository;
        private readonly ILogger<UploadModelActivity> _logger;
        private static readonly IReadOnlyList<string> _validFiles = new string[]
        {
            "app.py",
            "python-3.8.1.amd64",
        };

        public UploadModelActivity(IOption option, IModelStore modelRepository, ILogger<UploadModelActivity> logger)
        {
            _option = option;
            _modelRepository = modelRepository;
            _logger = logger;
        }

        public async Task Upload(CancellationToken token)
        {
            var modelId = new ModelId(_option.ModelName!, _option.VersionId!);

            _logger.LogInformation($"Uploading model {_option.PackageFile} to model {modelId}, force={_option.Force}");

            VerifyIsZip();
            await _modelRepository.Upload(_option.PackageFile!, modelId, _option.Force, token);
        }

        private void VerifyIsZip()
        {
            using Stream zipStream = new FileStream(_option.PackageFile!, FileMode.Open);
            using ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read, false);

            Func<string, string?> getRootName = x => x.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            zipArchive.Entries
                .Select(x => getRootName(x.FullName))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Join(_validFiles, x => x, x => x, (o, i) => (o, i))
                .Count()
                .VerifyAssert(x => x == 2, $"Zip file is missing the required file and folder {string.Join(", ", _validFiles)}");
        }
    }
}
