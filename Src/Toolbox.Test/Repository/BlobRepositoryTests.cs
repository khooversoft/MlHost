using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Repository;
using Toolbox.Test.Application;
using Toolbox.Tools;

namespace Toolbox.Test.Repository
{
    [TestClass]
    public class BlobRepositoryTests
    {
        private const string _testFile = "Test.json";

        [TestMethod]
        public async Task GivenData_WhenSaved_ShouldWork()
        {
            const string data = "this is a test";
            const string path = "testString.txt";

            IBlobRepository blobRepository = TestOption.GetBlobRepository();

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            await blobRepository.Write(path, dataBytes, CancellationToken.None);

            byte[] receive = await blobRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();

            Enumerable.SequenceEqual(dataBytes, receive).Should().BeTrue();

            BlobInfo? blobInfo = await blobRepository.GetBlobInfo(path, CancellationToken.None);
            blobInfo.Should().NotBeNull();
            blobInfo!.ContentHash.Should().NotBeNull();

            byte[] dataHash = dataBytes.ToMd5Hash();
            Enumerable.SequenceEqual(blobInfo.ContentHash, dataHash).Should().BeTrue();

            (await blobRepository.Exist(path, CancellationToken.None)).Should().BeTrue();
            await blobRepository.Delete(path, CancellationToken.None);
            (await blobRepository.Exist(path, CancellationToken.None)).Should().BeFalse();

            IReadOnlyList<BlobInfo> list = await blobRepository.Search(null!, x => true, CancellationToken.None);
            list.Should().NotBeNull();
            list.Count.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task GivenFile_WhenSaved_ShouldWork()
        {
            const string path = "Test.json";

            IBlobRepository blobRepository = TestOption.GetBlobRepository();

            string originalFilePath = FileTools.WriteResourceToTempFile(path, typeof(BlobRepositoryTests), TestOption._resourceId);
            originalFilePath.Should().NotBeNullOrEmpty();

            using (Stream readFile = new FileStream(originalFilePath, FileMode.Open))
            {
                await blobRepository.Upload(readFile, path, true, CancellationToken.None);
            }

            string downloadFilePath = Path.Combine(Path.GetDirectoryName(originalFilePath)!, path + ".downloaded");

            using (Stream writeFile = new FileStream(downloadFilePath, FileMode.Create))
            {
                await blobRepository.Download(path, writeFile, CancellationToken.None);
            }

            byte[] originalFileHash = FileTools.GetFileHash(originalFilePath);
            byte[] downloadFileHash = FileTools.GetFileHash(downloadFilePath);

            Enumerable.SequenceEqual(originalFileHash, downloadFileHash).Should().BeTrue();

            BlobInfo? blobInfo = await blobRepository.GetBlobInfo(path, CancellationToken.None);
            blobInfo.Should().NotBeNull();
            blobInfo!.ContentHash.Should().NotBeNull();
            Enumerable.SequenceEqual(blobInfo.ContentHash, originalFileHash).Should().BeTrue();

            (await blobRepository.Exist(path, CancellationToken.None)).Should().BeTrue();
            await blobRepository.Delete(path, CancellationToken.None);
            (await blobRepository.Exist(path, CancellationToken.None)).Should().BeFalse();
        }
    }
}
