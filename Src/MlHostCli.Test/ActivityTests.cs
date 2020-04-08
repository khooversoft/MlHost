using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHostApi.Types;
using MlHostCli.Activity;
using MlHostCli.Application;
using MlHostCli.Test.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Repository;
using Toolbox.Tools;

namespace MlHostCli.Test
{
    [TestClass]
    public class ActivityTests
    {
        [TestMethod]
        public async Task GivenZipModel_WhenUploaded_ShouldPass()
        {
            ModelFixture modelFixture = ModelFixture.GetModelFixture();

            string tempZipFile = FileTools.WriteResourceToTempFile("TestZip.Zip", nameof(ActivityTests), typeof(ActivityTests), "MlHostCli.Test.TestConfig.TestZip.zip");

            ModelId modelId = new ModelId($"test-zip-{Guid.NewGuid()}/v100");

            IOption option = new TestOption
            {
                PackageFile= tempZipFile,
                ModelName = modelId.ModelName,
                VersionId = modelId.VersionId,
            };

            await new UploadModelActivity(option, modelFixture.ModelRepository, modelFixture.Telemetry).Upload(CancellationToken.None);

            (await modelFixture.ModelRepository.Exist(modelId, CancellationToken.None)).Should().BeTrue();

            File.Delete(tempZipFile);
        }

        [TestMethod]
        public async Task GiveZipModelWhenUploaded_WhenDownloadAndDeleted_ShouldVerify()
        {
            ModelFixture modelFixture = ModelFixture.GetModelFixture();

            string tempZipFile = FileTools.WriteResourceToTempFile("TestZip.Zip", nameof(ActivityTests), typeof(ActivityTests), "MlHostCli.Test.TestConfig.TestZip.zip");

            ModelId modelId = new ModelId($"test-zip-{Guid.NewGuid()}/v100");

            IOption option = new TestOption
            {
                PackageFile = tempZipFile,
                ModelName = modelId.ModelName,
                VersionId = modelId.VersionId,
            };

            await new UploadModelActivity(option, modelFixture.ModelRepository, modelFixture.Telemetry).Upload(CancellationToken.None);

            string toZipFile = Path.GetDirectoryName(tempZipFile).Func(x => Path.Combine(x!, "TestZip-Copy.Zip"));

            IOption downloadOption = new TestOption
            {
                PackageFile = toZipFile,
                ModelName = option.ModelName,
                VersionId = option.VersionId,
            };

            await new DownloadModelActivity(downloadOption, modelFixture.ModelRepository, modelFixture.Telemetry).Download(CancellationToken.None);

            byte[] originalZipHash = FileTools.GetFileHash(tempZipFile);
            byte[] downloadZipHash = FileTools.GetFileHash(toZipFile);
            Enumerable.SequenceEqual(originalZipHash, downloadZipHash).Should().BeTrue();

            DeleteModelActivity uploadDeleteActivity = new DeleteModelActivity(option, modelFixture.ModelRepository, modelFixture.Telemetry);
            await uploadDeleteActivity.Delete(CancellationToken.None);

            (await modelFixture.ModelRepository.Exist(modelId, CancellationToken.None)).Should().BeFalse();

            File.Delete(tempZipFile);
            File.Delete(toZipFile);
        }
    }
}
