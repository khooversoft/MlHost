using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHostCli.Activity;
using MlHostCli.Application;
using MlHostCli.Test.Application;
using MlHostSdk.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHostCli.Test.Activity
{
    [TestClass]
    public class UploadDownloadActivityTests
    {
        private const string _testZipResourceId = "MlHostCli.Test.TestConfig.TestZip.zip";

        [TestCategory("Developer")]
        [TestMethod]
        public async Task GivenZipModel_WhenUploaded_ShouldPass()
        {
            ModelFixture modelFixture = ModelFixture.GetModelFixture();

            string tempZipFile = FileTools.WriteResourceToTempFile("TestZip.Zip", nameof(MlPackageActivityTests), typeof(MlPackageActivityTests), _testZipResourceId);

            ModelId modelId = new ModelId($"test-zip-{Guid.NewGuid()}/v100");

            IOption option = new Option
            {
                PackageFile = tempZipFile,
                ModelName = modelId.ModelName,
                VersionId = modelId.VersionId,
            };

            await new UploadModelActivity(option, modelFixture.ModelRepository, new NullLogger<UploadModelActivity>()).Upload(CancellationToken.None);

            (await modelFixture.ModelRepository.Exist(modelId, CancellationToken.None)).Should().BeTrue();

            File.Delete(tempZipFile);
        }

        [TestCategory("Developer")]
        [TestMethod]
        public async Task GiveZipModelWhenUploaded_WhenDownloadAndDeleted_ShouldVerify()
        {
            ModelFixture modelFixture = ModelFixture.GetModelFixture();

            string tempZipFile = FileTools.WriteResourceToTempFile("TestZip.Zip", nameof(MlPackageActivityTests), typeof(MlPackageActivityTests), _testZipResourceId);

            ModelId modelId = new ModelId($"test-zip-{Guid.NewGuid()}/v100");

            IOption option = new Option
            {
                PackageFile = tempZipFile,
                ModelName = modelId.ModelName,
                VersionId = modelId.VersionId,
            };

            await new UploadModelActivity(option, modelFixture.ModelRepository, new NullLogger<UploadModelActivity>())
                .Upload(CancellationToken.None);

            string toZipFile = Path.GetDirectoryName(tempZipFile).Func(x => Path.Combine(x!, "TestZip-Copy.Zip"));

            IOption downloadOption = new Option
            {
                PackageFile = toZipFile,
                ModelName = option.ModelName,
                VersionId = option.VersionId,
            };

            await new DownloadModelActivity(downloadOption, modelFixture.ModelRepository, new NullLogger<DownloadModelActivity>()).Download(CancellationToken.None);

            byte[] originalZipHash = FileTools.GetFileHash(tempZipFile);
            byte[] downloadZipHash = FileTools.GetFileHash(toZipFile);
            originalZipHash.SequenceEqual(downloadZipHash).Should().BeTrue();

            DeleteModelActivity uploadDeleteActivity = new DeleteModelActivity(option, modelFixture.ModelRepository, new NullLogger<DeleteModelActivity>());
            await uploadDeleteActivity.Delete(CancellationToken.None);

            (await modelFixture.ModelRepository.Exist(modelId, CancellationToken.None)).Should().BeFalse();

            File.Delete(tempZipFile);
            File.Delete(toZipFile);
        }
    }
}
