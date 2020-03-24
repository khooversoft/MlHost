using FluentAssertions;
using MlHostCli.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace MlHostCli.Test.Option
{
    public class OptionNegativeTests
    {
        [Fact]
        public void GivenInvalidUploadOption_WhenBuild_ShouldThrow()
        {
            var args = new string[]
            {
                    "upload",
                    "zipFile=zipFile1.zip-dkdkdk",
                    "modelName=ml-model-temp",
                    "VersionId=v10-0-0-1",
            };

            Action act = () => new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenInvalidDownloadOption_WhenBuild_ShouldThrow()
        {
            var args = new string[]
            {
                "DOWNLOAD",
                "zipFile=c:\\zipfile2.zip",
                "modelName=mYmodel99",  // no upper case
                "VersionId=x1000",
            };

            Action act = () => new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenInvalidDownloadOptionWhenModelNameIsMissing_WhenBuild_ShouldThrow()
        {
            var args = new string[]
            {
                "DOWNLOAD",
                "zipFile=c:\\zipfile2.zip",
                "VersionId=x1000",
            };

            Action act = () => new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            act.Should().Throw<ArgumentException>();
        }


        [Fact]
        public void GivenActivateOptionWithHostMissing_WhenBuild_ShouldThrow()
        {
            var args = new string[]
            {
                "Activate",
                "modelName=model-temp",
                "VersionId=v1000",
                "BlobStore:ContainerName=containerName",
                "BlobStore:ConnectionString=connectionString",
            };

            Action act = () => new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            act.Should().Throw<ArgumentException>();
        }
    }
}
