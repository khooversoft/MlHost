using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHostCli.Application;
using System.IO;
using Toolbox.Services;

namespace MlHostCli.Test.Option
{
    [TestClass]
    public class OptionPositiveTests
    {
        [TestMethod]
        public void GivenValidHelpOption_WhenBuild_ShouldSucceed()
        {
            var args = new string[]
            {
                "help"
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            option.Help.Should().BeTrue();
        }

        [TestMethod]
        public void GivenValidUploadOption_WhenBuild_ShouldSucceed()
        {
            var tempZipFile = Path.Combine(Path.GetTempPath(), "zipFile1.mlPackage");
            File.WriteAllText(tempZipFile, "hello");

            try
            {
                var args = new string[]
                {
                    "upload",
                    $"PackageFile={tempZipFile}",
                    "modelName=ml-model-temp",
                    "VersionId=v10-0-0-1",
                    "Store:ContainerName=containerName",
                    "Store:AccountName=accountName",
                    "Store:AccountKey=dummyKey",
                };

                IOption option = new OptionBuilder()
                    .AddCommandLine(args)
                    .Build();

                option.Upload.Should().BeTrue();
                option.PackageFile.Should().Be(tempZipFile);
                option.ModelName.Should().Be("ml-model-temp");
                option.VersionId.Should().Be("v10-0-0-1");

                option.Store.Should().NotBeNull();
                option.Store!.ContainerName.Should().Be("containerName");
                option.Store!.AccountName.Should().Be("accountName");
                option.Store!.AccountKey.Should().Be("dummyKey");
            }
            finally
            {
                File.Delete(tempZipFile);
            }
        }

        [TestMethod]
        public void GivenValidDownloadOptionWithConfigFile_WhenBuild_ShouldSucceed()
        {
            var tempConfigFile = Path.Combine(Path.GetTempPath(), "test-configfile.json");

            IJson json = new Json();

            dynamic config = new
            {
                Download = true,
                PackageFile = "c:\\zipfile2.mlPackage",
                ModelName = "ml-model-temp",
                VersionID = "v10-0-0-1",
                Store = new
                {
                    ContainerName = "containerName",
                    AccountName = "accountName",
                    AccountKey = "accountKey",
                },
            };

            string data = json.Serialize(config);
            File.WriteAllText(tempConfigFile, data);

            try
            {
                var args = new string[]
                {
                    $"configfile={tempConfigFile}",
                    "ModelName=ml-model-temp-next-version",
                };

                IOption option = new OptionBuilder()
                    .AddCommandLine(args)
                    .Build();

                option.Download.Should().BeTrue();
                option.PackageFile.Should().Be("c:\\zipfile2.mlPackage");
                option.ModelName.Should().Be("ml-model-temp-next-version");
                option.VersionId.Should().Be("v10-0-0-1");

                option.Store.Should().NotBeNull();
                option.Store!.ContainerName.Should().Be("containerName");
                option.Store!.AccountName.Should().Be("accountName");
                option.Store!.AccountKey.Should().Be("accountKey");
            }
            finally
            {
                File.Delete(tempConfigFile);
            }
        }

        [TestMethod]
        public void GivenValidDownloadOption_WhenBuild_ShouldSucceed()
        {
            var args = new string[]
            {
                "DOWNLOAD",
                "PackageFile=c:\\zipfile2.mlPackage",
                "modelName=mymodel99",
                "VersionId=x1000",
                "Store:ContainerName=containerName",
                "Store:AccountName=accountName",
                "Store:AccountKey=dummyKey",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            option.Download.Should().BeTrue();
            option.PackageFile.Should().Be("c:\\zipfile2.mlPackage");
            option.ModelName.Should().Be("mymodel99");
            option.VersionId.Should().Be("x1000");

            option.Store.Should().NotBeNull();
            option.Store!.ContainerName.Should().Be("containerName");
            option.Store!.AccountName.Should().Be("accountName");
            option.Store!.AccountKey.Should().Be("dummyKey");
        }

        [TestMethod]
        public void GivenValidDeleteOption_WhenBuild_ShouldSucceed()
        {
            var args = new string[]
            {
                "Delete",
                "modelName=mymodel-temp",
                "VersionId=v1000",
                "Store:ContainerName=containerName",
                "Store:AccountName=accountName",
                "Store:AccountKey=dummyKey",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            option.Delete.Should().BeTrue();
            option.ModelName.Should().Be("mymodel-temp");
            option.VersionId.Should().Be("v1000");

            option.Store.Should().NotBeNull();
            option.Store!.ContainerName.Should().Be("containerName");
            option.Store!.AccountName.Should().Be("accountName");
            option.Store!.AccountKey.Should().Be("dummyKey");
        }

        [TestMethod]
        public void GivenValidBindOption_WhenBuild_ShouldSucceed()
        {
            var args = new string[]
            {
                "Bind",
                "modelName=model-temp",
                "VersionId=v1000",
                "InstallPath=c:\\folder\\installPath",
                "Store:ContainerName=containerName",
                "Store:AccountName=accountName",
                "Store:AccountKey=dummyKey",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            option.ModelName.Should().Be("model-temp");
            option.VersionId.Should().Be("v1000");

            option.Store.Should().NotBeNull();
            option.Store!.ContainerName.Should().Be("containerName");
            option.Store!.AccountName.Should().Be("accountName");
            option.Store!.AccountKey.Should().Be("dummyKey");
        }
    }
}
