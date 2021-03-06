﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHost.Models;
using MlHost.Test.Application;
using MlHostSdk.Models;
using MlHostSdk.RestApi;
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
            ExecutionState.Running.ToString(),
        };

        [TestMethod]
        public async Task GivenMlHost_WhenPing_ShouldResponed()
        {
            TestWebsiteHost host = await TestApplication.GetHost();
            IJson jsonSerializer = host.Resolve<IJson>();

            var response = await host.Client.GetAsync("api/ping");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            PingResponse pingResponse = jsonSerializer.Deserialize<PingResponse>(responseString);

            pingResponse.Should().NotBeNull();
            pingResponse.Status.Should().NotBeNullOrEmpty();
            pingResponse.Status.Func(x => _validResponses.Contains(x!)).Should().BeTrue();
        }

        [TestMethod]
        public async Task GivenTestModel_WhenGetLogs_ShouldResponsed()
        {
            TestWebsiteHost host = await TestApplication.GetHost();
            await host.WaitForStartup();

            PingLogs pingLogs = await new ModelRestApi(host.Client).GetLogs();
            pingLogs.Should().NotBeNull();
            pingLogs.Messages.Should().NotBeNull();
            pingLogs.Messages!.Count.Should().BeGreaterThan(0);
        }
    }
}
