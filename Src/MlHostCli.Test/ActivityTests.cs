using FluentAssertions;
using MlHostApi.Tools;
using MlHostCli.Activity;
using MlHostCli.Application;
using MlHostCli.Test.Application;
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
            await _modelFixture.ClearAllBlob();

            string tempZipFile = WriteResourceToFile("TestZip.Zip", "MlHostCli.Test.TestConfig.TestZip.zip");

            IOption option = new TestOption
            {
                ZipFile= tempZipFile,
                ModelName = "test-zip",
                VersionId = "v100",
            };

            await new UploadModelActivity(option, _modelFixture.ModelRepository).Upload(CancellationToken.None);

            IReadOnlyList<string> blobList = await _modelFixture.ModelRepository.Search(string.Empty, "*", CancellationToken.None);
            blobList.Should().NotBeNull();
            blobList.Count.Should().Be(1);

            File.Delete(tempZipFile);
            await _modelFixture.ClearAllBlob();
        }

        [Fact]
        public async Task GiveZipModelWhenUploaded_WhenDownloadAndDeleted_ShouldVerify()
        {
            await _modelFixture.ClearAllBlob();

            string tempZipFile = WriteResourceToFile("TestZip.Zip", "MlHostCli.Test.TestConfig.TestZip.zip");

            IOption option = new TestOption
            {
                ZipFile = tempZipFile,
                ModelName = "test-zip",
                VersionId = "v100",
            };

            await new UploadModelActivity(option, _modelFixture.ModelRepository).Upload(CancellationToken.None);


            string toZipFile = Path.GetDirectoryName(tempZipFile).Do(x => Path.Combine(x!, "TestZip-Copy.Zip"));

            IOption downloadOption = new TestOption
            {
                ZipFile = toZipFile,
                ModelName = "test-zip",
                VersionId = "v100",
            };

            await new DownloadModelActivity(downloadOption, _modelFixture.ModelRepository).Download(CancellationToken.None);

            byte[] originalZipHash = GetFileHash(tempZipFile);
            byte[] downloadZipHash = GetFileHash(toZipFile);

            Enumerable.SequenceEqual(originalZipHash, downloadZipHash);

            DeleteModelActivity uploadDeleteActivity = new DeleteModelActivity(option, _modelFixture.ModelRepository);
            await uploadDeleteActivity.Delete(CancellationToken.None);

            IReadOnlyList<string> blobList = await _modelFixture.ModelRepository.Search(string.Empty, "*", CancellationToken.None);
            blobList.Should().NotBeNull();
            blobList.Count.Should().Be(0);

            File.Delete(tempZipFile);
            File.Delete(toZipFile);
            await _modelFixture.ClearAllBlob();
        }
    }
}
