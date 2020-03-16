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
                "ForceDeployment=false",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            option.Should().NotBeNull();
            option.ServiceUri.Should().Be(uri);
            option.ForceDeployment.Should().BeFalse();
        }

        [Fact]
        public void GivenOptionWithForceTrue_WhenBuild_ShouldPass()
        {
            const string uri = "http://myml/question";

            string[] args = new[]
            {
                $"ServiceUri={uri}",
                "ForceDeployment=true",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            option.Should().NotBeNull();
            option.ServiceUri.Should().Be(uri);
            option.ForceDeployment.Should().BeTrue();
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
