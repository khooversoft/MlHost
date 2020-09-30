using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHostCli.Application;
using System;

namespace MlHostCli.Test.Application
{
    [TestClass]
    public class OptionNegativeTests
    {
        [TestMethod]
        public void GivenInvalidUploadOption_WhenBuild_ShouldThrow()
        {
            var args = new string[]
            {
                    "upload",
                    "PackageFile=zipFile1.zip-dkdkdk",
                    "modelName=ml-model-temp",
                    "VersionId=v10-0-0-1",
            };

            Action act = () => new OptionBuilder()
                .SetArgs(args)
                .Build();

            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void GivenInvalidDownloadOption_WhenBuild_ShouldThrow()
        {
            var args = new string[]
            {
                "DOWNLOAD",
                "PackageFile=c:\\zipfile2.zip",
                "modelName=mYmodel99",  // no upper case
                "VersionId=x1000",
            };

            Action act = () => new OptionBuilder()
                .SetArgs(args)
                .Build();

            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void GivenInvalidDownloadOptionWhenModelNameIsMissing_WhenBuild_ShouldThrow()
        {
            var args = new string[]
            {
                "DOWNLOAD",
                "PackageFile=c:\\zipfile2.zip",
                "VersionId=x1000",
            };

            Action act = () => new OptionBuilder()
                .SetArgs(args)
                .Build();

            act.Should().Throw<ArgumentException>();
        }


        [TestMethod]
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
                .SetArgs(args)
                .Build();

            act.Should().Throw<ArgumentException>();
        }
    }
}
