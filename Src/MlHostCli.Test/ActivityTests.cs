using FluentAssertions;
using MlHostApi.Tools;
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
using Xunit;
using static MlHostCli.Test.Application.Function;

namespace MlHostCli.Test
{
    public class ActivityTests : IClassFixture<ModelFixture>
    {
        private readonly ModelFixture _modelFixture;

        public ActivityTests(ModelFixture modelFixture)
        {
            _modelFixture = modelFixture;
        }

        [Fact]
        public async Task GivenZipModel_WhenUploaded_ShouldPass()
        {
            string tempZipFile = WriteResourceToFile("TestZip.Zip", "MlHostCli.Test.TestConfig.TestZip.zip");

            ModelId modelId = new ModelId($"test-zip-{Guid.NewGuid()}/v100");

            IOption option = new TestOption
            {
                ZipFile= tempZipFile,
                ModelName = modelId.ModelName,
                VersionId = modelId.VersionId,
            };

            await new UploadModelActivity(option, _modelFixture.ModelRepository, _modelFixture.Telemetry).Upload(CancellationToken.None);

            IReadOnlyList<string> blobList = await _modelFixture.ModelRepository.Search(modelId, "*", CancellationToken.None);
            blobList.Should().NotBeNull();
            blobList.Count.Should().Be(1);

            File.Delete(tempZipFile);
        }

        [Fact]
        public async Task GiveZipModelWhenUploaded_WhenDownloadAndDeleted_ShouldVerify()
        {
            string tempZipFile = WriteResourceToFile("TestZip.Zip", "MlHostCli.Test.TestConfig.TestZip.zip");

            ModelId modelId = new ModelId($"test-zip-{Guid.NewGuid()}/v100");

            IOption option = new TestOption
            {
                ZipFile = tempZipFile,
                ModelName = modelId.ModelName,
                VersionId = modelId.VersionId,
            };

            await new UploadModelActivity(option, _modelFixture.ModelRepository, _modelFixture.Telemetry).Upload(CancellationToken.None);

            string toZipFile = Path.GetDirectoryName(tempZipFile).Func(x => Path.Combine(x!, "TestZip-Copy.Zip"));

            IOption downloadOption = new TestOption
            {
                ZipFile = toZipFile,
                ModelName = option.ModelName,
                VersionId = option.VersionId,
            };

            await new DownloadModelActivity(downloadOption, _modelFixture.ModelRepository, _modelFixture.Telemetry).Download(CancellationToken.None);

            byte[] originalZipHash = GetFileHash(tempZipFile);
            byte[] downloadZipHash = GetFileHash(toZipFile);
            Enumerable.SequenceEqual(originalZipHash, downloadZipHash).Should().BeTrue();

            DeleteModelActivity uploadDeleteActivity = new DeleteModelActivity(option, _modelFixture.ModelRepository, _modelFixture.Telemetry);
            await uploadDeleteActivity.Delete(CancellationToken.None);

            IReadOnlyList<string> blobList = await _modelFixture.ModelRepository.Search(modelId, "*", CancellationToken.None);
            blobList.Should().NotBeNull();
            blobList.Count.Should().Be(0);

            File.Delete(tempZipFile);
            File.Delete(toZipFile);
        }
    }
}
