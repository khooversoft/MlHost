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
        private readonly string _modelName;

        public TestFakeMlHost(string modelName)
        {
            modelName.VerifyNotEmpty(nameof(modelName));

            _modelName = modelName;
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
                $"ModelName={_modelName}",
            };

            return new OptionBuilder()
                .SetArgs(args)
                .Build();
        }
    }
}
