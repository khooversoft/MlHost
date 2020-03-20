using FluentAssertions;
using MlHostApi.Services;
using MlHostCli.Application;
using System;
using System.IO;
using Xunit;

namespace MlHostCli.Test.Option
{
    public class OptionPositiveTests
    {
        [Fact]
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

        [Fact]
        public void GivenValidUploadOption_WhenBuild_ShouldSucceed()
        {
            var tempZipFile = Path.Combine(Path.GetTempPath(), "zipFile1.zip");
            File.WriteAllText(tempZipFile, "hello");

            try
            {
                var args = new string[]
                {
                    "upload",
                    $"zipFile={tempZipFile}",
                    "modelName=ml-model-temp",
                    "VersionId=v10-0-0-1",
                    "BlobStore:ContainerName=containerName",
                    "BlobStore:ConnectionString=connectionString",
                };

                IOption option = new OptionBuilder()
                    .AddCommandLine(args)
                    .Build();

                option.Upload.Should().BeTrue();
                option.ZipFile.Should().Be(tempZipFile);
                option.ModelName.Should().Be("ml-model-temp");
                option.VersionId.Should().Be("v10-0-0-1");

                option.BlobStore.Should().NotBeNull();
                option.BlobStore!.ContainerName.Should().Be("containerName");
                option.BlobStore!.ConnectionString.Should().Be("connectionString");
            }
            finally
            {
                File.Delete(tempZipFile);
            }
        }


        [Fact]
        public void GivenValidDownloadOptionWithConfigFile_WhenBuild_ShouldSucceed()
        {
            var tempConfigFile = Path.Combine(Path.GetTempPath(), "test-configfile.json");

            IJson json = new Json();

            dynamic config = new
            {
                Download = true,
                zipFile = "c:\\zipfile2.zip",
                ModelName = "ml-model-temp",
                VersionID = "v10-0-0-1",
                BlobStore = new
                {
                    ContainerName = "containerName",
                    ConnectionString = "connectionString",
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
                option.ZipFile.Should().Be("c:\\zipfile2.zip");
                option.ModelName.Should().Be("ml-model-temp-next-version");
                option.VersionId.Should().Be("v10-0-0-1");

                option.BlobStore.Should().NotBeNull();
                option.BlobStore!.ContainerName.Should().Be("containerName");
                option.BlobStore!.ConnectionString.Should().Be("connectionString");
            }
            finally
            {
                File.Delete(tempConfigFile);
            }
        }

        [Fact]
        public void GivenValidDownloadOption_WhenBuild_ShouldSucceed()
        {
            var args = new string[]
            {
                "DOWNLOAD",
                "zipFile=c:\\zipfile2.zip",
                "modelName=mymodel99",
                "VersionId=x1000",
                "BlobStore:ContainerName=containerName",
                "BlobStore:ConnectionString=connectionString",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            option.Download.Should().BeTrue();
            option.ZipFile.Should().Be("c:\\zipfile2.zip");
            option.ModelName.Should().Be("mymodel99");
            option.VersionId.Should().Be("x1000");

            option.BlobStore.Should().NotBeNull();
            option.BlobStore!.ContainerName.Should().Be("containerName");
            option.BlobStore!.ConnectionString.Should().Be("connectionString");
        }

        [Fact]
        public void GivenValidDeleteOption_WhenBuild_ShouldSucceed()
        {
            var args = new string[]
            {
                "Delete",
                "modelName=mymodel-temp",
                "VersionId=v1000",
                "BlobStore:ContainerName=containerName",
                "BlobStore:ConnectionString=connectionString",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            option.Delete.Should().BeTrue();
            option.ModelName.Should().Be("mymodel-temp");
            option.VersionId.Should().Be("v1000");

            option.BlobStore.Should().NotBeNull();
            option.BlobStore!.ContainerName.Should().Be("containerName");
            option.BlobStore!.ConnectionString.Should().Be("connectionString");
        }

        [Fact]
        public void GivenValidActivateOption_WhenBuild_ShouldSucceed()
        {
            var args = new string[]
            {
                "Activate",
                "modelName=model-temp",
                "VersionId=v1000",
                "HostName=Host-SVR1000",
                "BlobStore:ContainerName=containerName",
                "BlobStore:ConnectionString=connectionString",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            option.Activate.Should().BeTrue();
            option.ModelName.Should().Be("model-temp");
            option.VersionId.Should().Be("v1000");
            option.HostName.Should().Be("Host-SVR1000");

            option.BlobStore.Should().NotBeNull();
            option.BlobStore!.ContainerName.Should().Be("containerName");
            option.BlobStore!.ConnectionString.Should().Be("connectionString");
        }
    }
}
