using Microsoft.Extensions.Logging.Abstractions;
using MlHostCli.Activity;
using MlHostCli.Application;
using MlHostSdk.Models;
using MlHostSdk.Package;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Models;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostCli.Test.Tools
{
    internal class BuildTestMlPackage
    {
        private const string _pythonFolderText = "python-3.8.1.amd64";
        private const string _appText = "app.py";
        private const string _metadataText = "metadata.json";
        private const string _testZipResourceId = "MlHostCli.Test.TestConfig.TestZip.zip";

        public async Task<string> Build()
        {
            string testFileFolder = typeof(BuildTestMlPackage).ExtractZipFromResource(_testZipResourceId, nameof(BuildTestMlPackage), CancellationToken.None);

            // Build "ml spec file"
            string specFilePath = Path.Combine(testFileFolder, "test.mlPackageSpec.json");
            string mlPackageFilePath = Path.Combine(testFileFolder, "fake-model.mlPackage");

            const string pythonFolderText = "python-3.8.1.amd64";
            const string appText = "app.py";
            const string metadataText = "metadata.json";

            var spec = new MlPackageSpec
            {
                PackageFile = mlPackageFilePath,
                Manifest = new MlPackageManifest
                {
                    ModelName = "test",
                    VersionId = "test_model_v1",
                    RunCmd = "bin\\FakeModelServer.exe"
                },
                Copy = new[]
                {
                    new CopyTo { Source = Path.Combine(testFileFolder, "bin\\*.*"), Destination = "bin" },
                    new CopyTo { Source = Path.Combine(testFileFolder, $"{pythonFolderText}\\*.*"), Destination = pythonFolderText },
                    new CopyTo { Source = Path.Combine(testFileFolder, appText), Destination = appText },
                    new CopyTo { Source = Path.Combine(testFileFolder, metadataText), Destination = metadataText },
                }
            };

            await spec.WriteToFile(specFilePath, Json.Default);

            var args = new[]
            {
                "build",
                $"SpecFile={specFilePath}",
            };

            IOption option = new OptionBuilder()
                .SetArgs(args)
                .Build();

            await new BuildActivity(option, new NullLogger<BuildActivity>())
                .Build(CancellationToken.None);

            return mlPackageFilePath;
        }

        public IReadOnlyList<string> GetExportedFileList() => new[]
        {
            "bin\\FakeModelServer.exe",
            $"{_pythonFolderText}\\TestFile1.txt",
            $"{_pythonFolderText}\\TestFile2.txt",
            _appText,
            _metadataText,
            "mlPackage.manifest.json"
        };
    }
}
