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
            const string packageFile = "packageFileData";

            string[] args = new[]
            {
                $"ServiceUri={uri}",
                $"PackageFile={packageFile}",
            };

            IOption option = new OptionBuilder()
                .SetArgs(args)
                .Build();

            option.Should().NotBeNull();
            option.ServiceUri.Should().Be(uri);
            option.PackageFile.Should().Be(packageFile);
        }
    }
}
