using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHostApi.Models;
using MlHostApi.Types;
using MlHostCli.Activity;
using MlHostCli.Application;
using MlHostCli.Test.Application;
using System;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHostCli.Test
{
    [TestClass]
    public class ActivitationActivityTests
    {
        [TestMethod]
        public async Task GivenModel_WhenActivatedThenDeactivate_ShouldPass()
        {
            ModelFixture modelFixture = ModelFixture.GetModelFixture();

            await modelFixture.ModelRepository.WriteConfiguration(new HostConfigurationModel(), CancellationToken.None);

            string tempZipFile = FileTools.WriteResourceToTempFile("TestZip.Zip", typeof(ActivitationActivityTests), "MlHostCli.Test.TestConfig.TestZip.zip");

            IOption option = new TestOption
            {
                PackageFile = tempZipFile,
                ModelName = $"test-zip-{Guid.NewGuid()}",
                VersionId = "v100",
                HostName = "hostName",
            };

            await new UploadModelActivity(option, modelFixture.ModelRepository, modelFixture.Telemetry).Upload(CancellationToken.None);

            HostConfigurationModel hostConfigurationModel = await modelFixture.ModelRepository.ReadConfiguration(CancellationToken.None);
            hostConfigurationModel.Should().NotBeNull();
            hostConfigurationModel.HostAssignments.Should().NotBeNull();
            hostConfigurationModel.HostAssignments!.Count.Should().Be(0);

            await new ActivateModelActivity(option, modelFixture.ModelRepository, modelFixture.Telemetry).Activate(CancellationToken.None);

            hostConfigurationModel = await modelFixture.ModelRepository.ReadConfiguration(CancellationToken.None);
            hostConfigurationModel.Should().NotBeNull();
            hostConfigurationModel.HostAssignments.Should().NotBeNull();
            hostConfigurationModel.HostAssignments!.Count.Should().Be(1);
            hostConfigurationModel.HostAssignments![0].HostName.Should().Be(option.HostName);
            hostConfigurationModel.HostAssignments![0].ModelId.Should().Be(new ModelId(option.ModelName!, option.VersionId!));

            await new DeactivateModelActivity(option, modelFixture.ModelRepository, modelFixture.Telemetry).Deactivate(CancellationToken.None);

            hostConfigurationModel = await modelFixture.ModelRepository.ReadConfiguration(CancellationToken.None);
            hostConfigurationModel.Should().NotBeNull();
            hostConfigurationModel.HostAssignments.Should().NotBeNull();
            hostConfigurationModel.HostAssignments!.Count.Should().Be(0);
        }
    }
}
