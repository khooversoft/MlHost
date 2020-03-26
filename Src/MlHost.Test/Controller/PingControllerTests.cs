using FluentAssertions;
using MlHost.Services;
using MlHost.Test.Application;
using MlHostApi.Models;
using MlHostApi.Services;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MlHost.Test.Controller
{
    [Collection("WebsiteTest")]
    public class PingControllerTests : IClassFixture<TestHostWithStorage>
    {
        private readonly TestHostWithStorage _storageStoreFixture;

        private static HashSet<string> _validResponses = new HashSet<string>
        {
            ExecutionState.Booting.ToString(),
            ExecutionState.Starting.ToString(),
            ExecutionState.Deploying.ToString(),
            ExecutionState.Running.ToString(),
        };

        public PingControllerTests(TestHostWithStorage storageStoreFixture)
        {
            _storageStoreFixture = storageStoreFixture;
        }

        [Fact]
        public async Task GivenMlHost_WhenPing_ShouldResponed()
        {
            IJson jsonSerializer = _storageStoreFixture.Resolve<IJson>();

            var response = await _storageStoreFixture.Client.GetAsync("api/ping");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            PingResponse pingResponse = jsonSerializer.Deserialize<PingResponse>(responseString);

            pingResponse.Should().NotBeNull();
            pingResponse.Status.Should().NotBeNullOrEmpty();
            pingResponse.Status.Func(x => _validResponses.Contains(x!)).Should().BeTrue();
        }
    }
}
