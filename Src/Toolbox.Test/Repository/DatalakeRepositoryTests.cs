using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
    public class DatalakeRepositoryTests
    {
        [TestMethod]
        public async Task GivenData_WhenSaved_ShouldWork()
        {
            const string data = "this is a test";
            const string path = "testString.txt";

            IDatalakeRepository datalakeRepository = TestOption.GetDatalakeRepository();

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            await datalakeRepository.Write(path, dataBytes, true, CancellationToken.None);

            byte[] receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();

            Enumerable.SequenceEqual(dataBytes, receive).Should().BeTrue();

            DatalakePathProperties? pathProperties = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties.Should().NotBeNull();
            pathProperties!.ETag.Should().NotBeNullOrEmpty();

            (await datalakeRepository.Exist(path, CancellationToken.None)).Should().BeTrue();
            (await datalakeRepository.GetPathProperties(path, CancellationToken.None)).Should().NotBeNull();

            await datalakeRepository.Delete(path, CancellationToken.None);
            (await datalakeRepository.Exist(path, CancellationToken.None)).Should().BeFalse();

            IReadOnlyList<DatalakePathItem> list = await datalakeRepository.Search(null!, x => true, true, CancellationToken.None);
            list.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GivenFile_WhenSaved_ShouldWork()
        {
            const string path = "Test.json";

            IDatalakeRepository datalakeRepository = TestOption.GetDatalakeRepository();

            string originalFilePath = FileTools.WriteResourceToTempFile(path, typeof(DatalakeRepositoryTests), TestOption._resourceId);
            originalFilePath.Should().NotBeNullOrEmpty();

            using (Stream readFile = new FileStream(originalFilePath, FileMode.Open))
            {
                await datalakeRepository.Upload(readFile, path, true, CancellationToken.None);
            }

            string downloadFilePath = Path.Combine(Path.GetDirectoryName(originalFilePath)!, path + ".downloaded");

            using (Stream writeFile = new FileStream(downloadFilePath, FileMode.Create))
            {
                await datalakeRepository.Download(path, writeFile, CancellationToken.None);
            }

            byte[] originalFileHash = FileTools.GetFileHash(originalFilePath);
            byte[] downloadFileHash = FileTools.GetFileHash(downloadFilePath);

            Enumerable.SequenceEqual(originalFileHash, downloadFileHash).Should().BeTrue();

            DatalakePathProperties? pathProperties = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties.Should().NotBeNull();
            pathProperties!.ETag.Should().NotBeNullOrEmpty();

            (await datalakeRepository.Exist(path, CancellationToken.None)).Should().BeTrue();
            (await datalakeRepository.GetPathProperties(path, CancellationToken.None)).Should().NotBeNull();

            await datalakeRepository.Delete(path, CancellationToken.None);
            (await datalakeRepository.Exist(path, CancellationToken.None)).Should().BeFalse();
        }

        [TestMethod]
        public async Task GivenFiles_WhenSearched_ReturnsCorrectly()
        {
            const string path = "Test.json";

            IDatalakeRepository datalakeRepository = TestOption.GetDatalakeRepository();

            await ClearContainer(datalakeRepository);

            IReadOnlyList<DatalakePathItem> verifyList = await datalakeRepository.Search(null!, x => true, true, CancellationToken.None);
            verifyList.Should().NotBeNull();
            verifyList.Count.Should().Be(0);

            string originalFilePath = FileTools.WriteResourceToTempFile(path, typeof(DatalakeRepositoryTests), TestOption._resourceId);
            originalFilePath.Should().NotBeNullOrEmpty();

            string[] fileLists = new[]
            {
                "test1.json",
                "test2.json",
                "data/test3.json",
                "data/test4.json",
                "data2/test5.json"
            };

            int folderCount = fileLists
                .Select(x => (x, vectors: x.Split("/")))
                .Where(x => x.vectors.Length > 1)
                .Select(x => x.vectors[0])
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            foreach (var filePath in fileLists)
            {
                using (Stream readFile = new FileStream(originalFilePath, FileMode.Open))
                {
                    await datalakeRepository.Upload(readFile, filePath, true, CancellationToken.None);
                }
            }

            IReadOnlyList<DatalakePathItem> subSearchList = await datalakeRepository.Search("data", x => true, true, CancellationToken.None);
            subSearchList.Should().NotBeNull();
            subSearchList.Count.Should().Be(fileLists.Where(x => x.StartsWith("data/")).Count());

            IReadOnlyList<DatalakePathItem> searchList = await datalakeRepository.Search(null!, x => true, true, CancellationToken.None);
            searchList.Should().NotBeNull();
            searchList.Where(x => x.IsDirectory == false).Count().Should().Be(fileLists.Length);
            searchList.Where(x => x.IsDirectory == true).Count().Should().Be(folderCount);

            await ClearContainer(datalakeRepository);

            searchList = await datalakeRepository.Search(null!, x => true, true, CancellationToken.None);
            searchList.Should().NotBeNull();
            searchList.Count.Should().Be(0);
        }

        private async Task ClearContainer(IDatalakeRepository datalakeRepository)
        {
            IReadOnlyList<DatalakePathItem> list = await datalakeRepository.Search(null!, x => true, false, CancellationToken.None);
            list.Should().NotBeNull();

            foreach (var fileItem in list.Where(x => x.IsDirectory == true))
            {
                await datalakeRepository.DeleteDirectory(fileItem.Name!, CancellationToken.None);
            }

            foreach (var fileItem in list.Where(x => x.IsDirectory == false))
            {
                await datalakeRepository.Delete(fileItem.Name!, CancellationToken.None);
            }
        }
    }
}
