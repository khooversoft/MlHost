using FluentAssertions;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Services;
using MlHost.Services.PackageSource;
using MlHost.Test.Application;
using MlHost.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MlHost.Test.Tools
{
    public class PackageUpdateTests
    {
        private readonly string[] _TestZipFiles = new[]
        {
            "TestFile1.txt",
            "TestFile2.txt",
            @"Folder1\TestFile1.txt",
            @"Folder1\TestFile2.txt",
        };

        private readonly string[] _TestZipFiles_Updated = new[]
{
            "TestFile1.txt",
            "TestFile3.txt",
            "Folder1\\TestFile2.txt",
            "Folder2\\TestFile12.txt",
            "Folder2\\TestFile22.txt",
        };

        [Fact]
        public async Task GivenEmbeddedZipFile_WhenDeployed_ShouldMatchFiles()
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), nameof(PackageUpdateTests));
            if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);

            var memoryLogger = new MemoryLogger<PackageDeployment>();
            IExecutionContext executionContext = new Services.ExecutionContext();

            string[] args = new[]
            {
                "ZipFileUri=notValid",
                "ForceDeployment=false",
                "BlobStore:ContainerName=notValid",
                "BlobStore:ConnectionString=notValid",
                $"Deployment:DeploymentFolder={tempFolder}",
                "Deployment:PackageFolder=notValid",
            };

            IOption option = new OptionBuilder()
                .SetArgs(args)
                .Build();

            IPackageSource originalPackageSource = new PackageSourceFromResource(typeof(PackageUpdateTests), "MlHost.Test.Package.TestZip.zip");
            IPackageDeployment originalDeployment = new PackageDeployment(memoryLogger, option, executionContext, originalPackageSource);
            await originalDeployment.Deploy();

            memoryLogger.Count().Should().BeGreaterThan(0);

            string[] files = Directory.GetFiles(tempFolder, "*.*", SearchOption.AllDirectories)
                .Select(x => x.Substring(tempFolder.Length + 1))
                .ToArray();

            files.Length.Should().Be(_TestZipFiles.Length);
            files.OrderBy(x => x)
                .Zip(_TestZipFiles.OrderBy(x => x), (o, i) => (o, i))
                .All(x => x.o.Equals(x.i, StringComparison.OrdinalIgnoreCase))
                .Should().BeTrue();

            IPackageSource updatePackageSource = new PackageSourceFromResource(typeof(PackageUpdateTests), "MlHost.Test.Package.TestZip-Update.zip");
            IPackageDeployment updateDeployment = new PackageDeployment(memoryLogger, option, executionContext, updatePackageSource);
            await updateDeployment.Deploy();

            memoryLogger.Count().Should().BeGreaterThan(0);

            string[] updateFiles = Directory.GetFiles(tempFolder, "*.*", SearchOption.AllDirectories)
                .Select(x => x.Substring(tempFolder.Length + 1))
                .ToArray();

            updateFiles.Length.Should().Be(_TestZipFiles_Updated.Length);
            updateFiles.OrderBy(x => x)
                .Zip(_TestZipFiles_Updated.OrderBy(x => x), (o, i) => (o, i))
                .All(x => x.o.Equals(x.i, StringComparison.OrdinalIgnoreCase))
                .Should().BeTrue();

            Directory.Delete(tempFolder, true);
        }
    }
}
