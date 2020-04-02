using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHost.Services;
using MlHost.Test.Application;
using MlHostApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHost.Test.Controller
{
    [TestClass]
    public class PingControllerTests
    {
        private static HashSet<string> _validResponses = new HashSet<string>
        {
            ExecutionState.Booting.ToString(),
            ExecutionState.Starting.ToString(),
            ExecutionState.Deploying.ToString(),
            ExecutionState.Running.ToString(),
        };

        [TestMethod]
        public async Task GivenMlHost_WhenPing_ShouldResponed()
        {
            var host = TestHostWithStorage.GetHost();

            IJson jsonSerializer = host.Resolve<IJson>();

            var response = await host.Client.GetAsync("api/ping");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            PingResponse pingResponse = jsonSerializer.Deserialize<PingResponse>(responseString);

            pingResponse.Should().NotBeNull();
            pingResponse.Status.Should().NotBeNullOrEmpty();
            pingResponse.Status.Func(x => _validResponses.Contains(x!)).Should().BeTrue();
        }
    }
}
