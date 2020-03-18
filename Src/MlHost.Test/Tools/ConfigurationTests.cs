using FluentAssertions;
using MlHost.Application;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MlHost.Test.Tools
{
    public class ConfigurationTests
    {
        [Fact]
        public void GivenOption_WhenBuild_ShouldPass()
        {
            const string uri = "http://0.0.0.0:8000/predict";

            string[] args = new[]
            {
                $"ServiceUri={uri}",
                "ZipFileUri=fileUri",
                "ForceDeployment=false",
                "BlobStore:ContainerName=containerName",
                "BlobStore:ConnectionString=connectionString",
                "Deployment:DeploymentFolder=deploymentFolder",
                "Deployment:PackageFolder=packageFolder",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            option.Should().NotBeNull();
            option.ServiceUri.Should().Be(uri);
            option.ForceDeployment.Should().BeFalse();
            option.ZipFileUri.Should().Be("fileUri");
            option.BlobStore.ContainerName.Should().Be("containerName");
            option.BlobStore.ConnectionString.Should().Be("connectionString");
            option.Deployment.DeploymentFolder.Should().Contain("deploymentFolder");
            option.Deployment.PackageFolder.Should().Contain("packageFolder");
        }

        [Fact]
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
