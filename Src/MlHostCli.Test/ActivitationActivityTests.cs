using FluentAssertions;
using MlHostApi.Models;
using MlHostApi.Types;
using MlHostCli.Activity;
using MlHostCli.Application;
using MlHostCli.Test.Application;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static MlHostCli.Test.Application.Function;

namespace MlHostCli.Test
{
    public class ActivitationActivityTests : IClassFixture<ModelFixture>
    {
        private readonly ModelFixture _modelFixture;

        public ActivitationActivityTests(ModelFixture modelFixture)
        {
            _modelFixture = modelFixture;
        }

        [Fact]
        public async Task GivenModel_WhenActivatedThenDeactivate_ShouldPass()
        {
            await _modelFixture.ModelRepository.WriteConfiguration(new HostConfigurationModel(), CancellationToken.None);

            string tempZipFile = WriteResourceToFile("TestZip.Zip", "MlHostCli.Test.TestConfig.TestZip.zip");

            IOption option = new TestOption
            {
                ZipFile = tempZipFile,
                ModelName = $"test-zip-{Guid.NewGuid()}",
                VersionId = "v100",
                HostName = "hostName",
            };

            await new UploadModelActivity(option, _modelFixture.ModelRepository, _modelFixture.Telemetry).Upload(CancellationToken.None);

            HostConfigurationModel hostConfigurationModel = await _modelFixture.ModelRepository.ReadConfiguration(CancellationToken.None);
            hostConfigurationModel.Should().NotBeNull();
            hostConfigurationModel.HostAssignments.Should().NotBeNull();
            hostConfigurationModel.HostAssignments!.Count.Should().Be(0);

            await new ActivateModelActivity(option, _modelFixture.ModelRepository, _modelFixture.Telemetry).Activate(CancellationToken.None);

            hostConfigurationModel = await _modelFixture.ModelRepository.ReadConfiguration(CancellationToken.None);
            hostConfigurationModel.Should().NotBeNull();
            hostConfigurationModel.HostAssignments.Should().NotBeNull();
            hostConfigurationModel.HostAssignments!.Count.Should().Be(1);
            hostConfigurationModel.HostAssignments![0].HostName.Should().Be(option.HostName);
            hostConfigurationModel.HostAssignments![0].ModelId.Should().Be(new ModelId(option.ModelName!, option.VersionId!));

            await new DeactivateModelActivity(option, _modelFixture.ModelRepository, _modelFixture.Telemetry).Deactivate(CancellationToken.None);

            hostConfigurationModel = await _modelFixture.ModelRepository.ReadConfiguration(CancellationToken.None);
            hostConfigurationModel.Should().NotBeNull();
            hostConfigurationModel.HostAssignments.Should().NotBeNull();
            hostConfigurationModel.HostAssignments!.Count.Should().Be(0);
        }
    }
}
