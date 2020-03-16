using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Models;
using MlHost.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Test.Application
{
    public class TestFixture : IDisposable
    {
        private IHost? _host;
        private HttpClient? _client;

        public IJson Json { get; } = new Json();

        public HttpClient Client => _client ?? throw new ArgumentNullException(nameof(Client));

        public TestFixture()
        {
            string[] args = new[]
            {
                "ServiceUri=http://localhost:5003/predict",
                "ForceDeployment=false",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .Build();

            var host = new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config
                        .AddUserSecrets("MlHost.Test");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(option)
                        .AddSingleton(Json);
                });

            _host = host.Start();
            _client = _host.GetTestServer().CreateClient();
        }

        public async Task WaitForStartup()
        {
            var token = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            while (!token.IsCancellationRequested)
            {
                var response = await Client.GetAsync("api/ping");
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                PingResponse result = Json.Deserialize<PingResponse>(responseString);

                if (result.Status == "Running") return;

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _client, null)?.Dispose();

            var host = Interlocked.Exchange(ref _host, null);
            if (host != null)
            {
                host.StopAsync(TimeSpan.FromSeconds(10)).Wait();
                host.Dispose();
            }
        }
    }
}
