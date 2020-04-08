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
                "DeploymentFolder=deploymentFolder",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            option.Should().NotBeNull();
            option.ServiceUri.Should().Be(uri);
            option.DeploymentFolder.Should().Contain("deploymentFolder");
        }
    }
}
