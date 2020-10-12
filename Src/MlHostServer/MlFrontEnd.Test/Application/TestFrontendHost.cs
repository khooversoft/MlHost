using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlFrontEnd.Application;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlFrontEnd.Test.Application
{
    internal class TestFrontendHost : TestHostBase
    {
        public TestFrontendHost() { }

        public TestFrontendHost StartApiServer(IEnumerable<KeyValuePair<string, TestFakeMlHost>> config)
        {
            config.VerifyNotNull(nameof(config));
            config.VerifyAssert(x => x.Count() > 0, $"{nameof(config)} is empty");

            List<KeyValuePair<string, TestFakeMlHost>> mlHosts = config.ToList();
            IOption option = GetOption(config);

            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(option)
                        .AddSingleton<IJson, Json>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<MlHostFrontEnd.Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton<IHttpClientFactory>(services => new TestHttpClientFactory(mlHosts));
                })
                .ConfigureLogging(builder => builder.AddDebug());

            _host = host.Start();
            _client = _host.GetTestServer().CreateClient();

            return this;
        }

        private IOption GetOption(IEnumerable<KeyValuePair<string, TestFakeMlHost>> config)
        {
            string[] args = config
                .SelectMany((x, i) => new[]
                {
                    $"Hosts:[{i}]:ModelName={x.Key}",
                    $"Hosts:[{i}]:Uri=http://localhost:{i + 4000:D4}"
                })
                .ToArray();

            return new OptionBuilder()
                .SetArgs(args.Append("environment=unknown").ToArray())
                .Build();
        }
    }
}
