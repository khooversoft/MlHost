using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Repository;
using Toolbox.Test.Application;

namespace Toolbox.Test.Repository
{
    [TestClass]
    public class DatalakeRepositoryEtagTests
    {
        [TestCategory("Developer")]
        [TestMethod]
        public async Task GivenData_WhenSaved_ShouldMatchEtag()
        {
            const string data = "this is a test";
            const string path = "testStringEtag.txt";

            IDatalakeRepository datalakeRepository = TestOption.GetDatalakeRepository();

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            await datalakeRepository.Write(path, dataBytes, true, CancellationToken.None);

            byte[] receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();
            Enumerable.SequenceEqual(dataBytes, receive).Should().BeTrue();

            DatalakePathProperties? pathProperties = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties.Should().NotBeNull();
            pathProperties!.ETag.Should().NotBeNullOrEmpty();

            receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();

            DatalakePathProperties? pathProperties2ndRead = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties2ndRead.Should().NotBeNull();
            pathProperties2ndRead!.ETag.Should().NotBeNullOrEmpty();

            pathProperties.ETag.Should().Be(pathProperties2ndRead.ETag);

            await datalakeRepository.Delete(path, CancellationToken.None);
        }

        [TestCategory("Developer")]
        [TestMethod]
        public async Task GivenData_WhenSavedAndWritten_ShouldNotMatchEtag()
        {
            const string data = "this is a test";
            const string path = "testStringEtag.txt";

            IDatalakeRepository datalakeRepository = TestOption.GetDatalakeRepository();

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            await datalakeRepository.Write(path, dataBytes, true, CancellationToken.None);

            byte[] receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();
            Enumerable.SequenceEqual(dataBytes, receive).Should().BeTrue();

            DatalakePathProperties? pathProperties = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties.Should().NotBeNull();
            pathProperties!.ETag.Should().NotBeNullOrEmpty();

            await datalakeRepository.Write(path, dataBytes, true, CancellationToken.None);
            receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();

            DatalakePathProperties? pathProperties2ndRead = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties2ndRead.Should().NotBeNull();
            pathProperties2ndRead!.ETag.Should().NotBeNullOrEmpty();

            pathProperties.ETag.Should().NotBe(pathProperties2ndRead.ETag);

            await datalakeRepository.Delete(path, CancellationToken.None);
        }

        [TestCategory("Developer")]
        [TestMethod]
        public async Task GivenData_WhenSavedAndUpdate_ShouldNotMatchEtag()
        {
            const string data = "this is a test";
            const string data2 = "this is a test2";
            const string path = "testStringEtag.txt";

            IDatalakeRepository datalakeRepository = TestOption.GetDatalakeRepository();

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            await datalakeRepository.Write(path, dataBytes, true, CancellationToken.None);

            byte[] receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();
            Enumerable.SequenceEqual(dataBytes, receive).Should().BeTrue();

            DatalakePathProperties? pathProperties = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties.Should().NotBeNull();
            pathProperties!.ETag.Should().NotBeNullOrEmpty();

            byte[] data2Bytes = Encoding.UTF8.GetBytes(data2);
            await datalakeRepository.Write(path, data2Bytes, true, CancellationToken.None);

            receive = await datalakeRepository.Read(path, CancellationToken.None);
            receive.Should().NotBeNull();

            DatalakePathProperties? pathProperties2ndRead = await datalakeRepository.GetPathProperties(path, CancellationToken.None);
            pathProperties2ndRead.Should().NotBeNull();
            pathProperties2ndRead!.ETag.Should().NotBeNullOrEmpty();

            pathProperties.ETag.Should().NotBe(pathProperties2ndRead.ETag);

            await datalakeRepository.Delete(path, CancellationToken.None);
            (await datalakeRepository.Exist(path, CancellationToken.None)).Should().BeFalse();
        }
    }
}
