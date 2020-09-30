using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlHost.Services;
using MlHostSdk.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;
using MlFrontEnd.Application;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MlFrontEnd.Test.Application
{
    internal class TestFrontendHost
    {
        protected IHost? _host;
        protected HttpClient? _client;

        public TestFrontendHost() { }

        public HttpClient HttpClient => _client ?? throw new ArgumentNullException(nameof(HttpClient));

        public T Resolve<T>() where T : class => _host?.Services.GetService<T>() ?? throw new InvalidOperationException($"Cannot find service {typeof(T).Name}");

        public TestFrontendHost StartApiServer(IEnumerable<KeyValuePair<string, TestMlHost>> config)
        {
            config.VerifyNotNull(nameof(config));
            config.VerifyAssert(x => x.Count() > 0, $"{nameof(config)} is empty");

            IOption option = GetOption(config.Select(x => x.Key));
            List<KeyValuePair<string, TestMlHost>> mlHosts = config.ToList();

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

        public void Shutdown()
        {
            Interlocked.Exchange(ref _client, null)?.Dispose();

            var host = Interlocked.Exchange(ref _host, null);
            if (host != null)
            {
                try
                {
                    host.StopAsync(TimeSpan.FromMinutes(10)).Wait();
                }
                catch { }
                finally
                {
                    host.Dispose();
                }
            }
        }

        private IOption GetOption(IEnumerable<string> versionIds)
        {
            string[] args = versionIds
                .SelectMany((x, i) => new[]
                {
                    $"Hosts:{i}]:VersionId=model{i}",
                    $"Hosts:[{i}]:Uri=http://localhost:{i + 4000:D4}"
                })
                .ToArray();

            //string[] args = new[]
            //{
            //    $"Hosts:[0]:VersionId={versionId}",
            //    "Hosts:[0]:Uri=http://localhost:4001"
            //};

            return new OptionBuilder()
                .SetArgs(args)
                .Build();
        }
    }
}
