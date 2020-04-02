using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHost.Application;
using System;

namespace MlHost.Test.Tools
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void GivenOption_WhenBuild_ShouldPass()
        {
            const string uri = "http://0.0.0.0:8000/predict";

            string[] args = new[]
            {
                $"ServiceUri={uri}",
                "HostName=hostName",
                "ForceDeployment=false",
                "BlobStore:ContainerName=containerName",
                "BlobStore:AccountName=accountName",
                "BlobStore:AccountKey=accountKey",
                "Deployment:DeploymentFolder=deploymentFolder",
                "Deployment:PackageFolder=packageFolder",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            option.Should().NotBeNull();
            option.ServiceUri.Should().Be(uri);
            option.ForceDeployment.Should().BeFalse();
            option.HostName.Should().Be("hostName");
            option.BlobStore!.ContainerName.Should().Be("containerName");
            option.BlobStore.AccountName.Should().Be("accountName");
            option.BlobStore.AccountKey.Should().Be("accountKey");
            option.Deployment.DeploymentFolder.Should().Contain("deploymentFolder");
            option.Deployment.PackageFolder.Should().Contain("packageFolder");
        }

        [TestMethod]
        public void GivenMissingOption_WhenBuild_ShouldThrowException()
        {
            string[] args = new[]
            {
                "ForceDeployment=false",
            };

            Action act = () => new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            act.Should().Throw<ArgumentException>();

        }
    }
}
