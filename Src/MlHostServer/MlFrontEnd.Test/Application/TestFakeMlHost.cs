using FakeMlHost.Application;
using FakeModelServer.Application;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Toolbox.Tools;

namespace MlFrontEnd.Test.Application
{
    internal class TestFakeMlHost : TestHostBase
    {
        private readonly string _versionId;

        public TestFakeMlHost(string versionId)
        {
            versionId.VerifyNotEmpty(nameof(versionId));

            _versionId = versionId;
        }

        public TestFakeMlHost StartApiServer()
        {
            IOption option = GetOption();

            var host = new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<FakeMlHost.Startup>();
                })
                .ConfigureLogging(builder => builder.AddDebug())
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(option);
                });

            _host = host.Start();
            _client = _host.GetTestServer().CreateClient();

            return this;
        }

        private IOption GetOption()
        {
            string[] args = new[]
            {
                $"VersionId={_versionId}",
            };

            return new OptionBuilder()
                .SetArgs(args)
                .Build();
        }
    }
}
