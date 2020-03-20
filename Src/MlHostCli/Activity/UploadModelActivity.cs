using MlHostApi.Repository;
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
    internal class UploadModelActivity
    {
        private readonly IOption _option;
        private readonly IModelRepository _modelRepository;

        private static readonly IReadOnlyList<string> _validFiles = new string[]
        {
            "app.py",
            "python-3.8.1.amd64",
        };

        public UploadModelActivity(IOption option, IModelRepository modelRepository)
        {
            _option = option;
            _modelRepository = modelRepository;
        }

        public async Task Upload(CancellationToken token)
        {
            var modelId = new ModelId(_option.ModelName, _option.VersionId);

            VerifyIsZip();
            await _modelRepository.Delete(modelId, token);
            await _modelRepository.Upload(_option.ZipFile, modelId, token);
        }

        private void VerifyIsZip()
        {
            using Stream zipStream = new FileStream(_option.ZipFile, FileMode.Open);
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
