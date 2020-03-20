using FluentAssertions;
using MlHostApi.Models;
using MlHostApi.Types;
using MlHostCli.Activity;
using MlHostCli.Application;
using MlHostCli.Test.Application;
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
        public async Task GivenModel_WhenActivated_ShouldPass()
        {
            await _modelFixture.ClearAllBlob();

            string tempZipFile = WriteResourceToFile("TestZip.Zip", "MlHostCli.Test.TestConfig.TestZip.zip");

            IOption option = new TestOption
            {
                ZipFile = tempZipFile,
                ModelName = "test-zip",
                VersionId = "v100",
                HostName = "hostName",
            };

            await new UploadModelActivity(option, _modelFixture.ModelRepository).Upload(CancellationToken.None);

            HostConfigurationModel testHostConfigurationModel = await _modelFixture.ModelRepository.ReadConfiguration(CancellationToken.None);
            testHostConfigurationModel.Should().NotBeNull();
            testHostConfigurationModel.HostAssignments.Should().NotBeNull();
            testHostConfigurationModel.HostAssignments!.Count.Should().Be(0);

            await new UploadModelActivity(option, _modelFixture.ModelRepository).Upload(CancellationToken.None);

            await new ActivateModelActivity(option, _modelFixture.ModelRepository).Activate(CancellationToken.None);

            HostConfigurationModel hostConfigurationModel = await _modelFixture.ModelRepository.ReadConfiguration(CancellationToken.None);
            hostConfigurationModel.Should().NotBeNull();
            hostConfigurationModel.HostAssignments.Should().NotBeNull();
            hostConfigurationModel.HostAssignments!.Count.Should().Be(1);
            hostConfigurationModel.HostAssignments![0].HostName.Should().Be(option.HostName);
            hostConfigurationModel.HostAssignments![0].ModelId.Should().Be(new ModelId(option.ModelName, option.VersionId));

            await _modelFixture.ClearAllBlob();
        }
    }
}
