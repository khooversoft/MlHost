using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHostSdk.Types;
using MlHostCli.Activity;
using MlHostCli.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Repository;
using Toolbox.Tools;
using MlHostSdk.Package;
using Toolbox.Models;
using Toolbox.Services;
using MlHostCli.Test.Tools;

namespace MlHostCli.Test.Activity
{
    [TestClass]
    public class MlPackageActivityTests
    {
        [TestCategory("Developer")]
        [TestMethod]
        public async Task GivenFiles_CreateMlPackage()
        {
            // Create files to package
            var builder = new BuildTestMlPackage();
            string mlPackageFilePath = await builder.Build();

            // Act
            mlPackageFilePath.VerifyAssert(x => File.Exists(x), $"File {mlPackageFilePath} does not exit");

            string extractToFolder = Path.Combine(Path.GetTempPath(), $"{nameof(MlPackageActivityTests)}_extract_{Guid.NewGuid()}");
            ZipArchiveTools.ExtractFromZipFile(mlPackageFilePath, extractToFolder, CancellationToken.None);

            // Assert
            builder.GetExportedFileList()
                .Select(x => Path.Combine(extractToFolder, x))
                .ForEach(x => x.VerifyAssert(y => File.Exists(x), $"File {x} does not exist after ML package was extract"));

            FileTools.DeleteDirectory(Path.GetDirectoryName(mlPackageFilePath)!);
        }
    }
}
