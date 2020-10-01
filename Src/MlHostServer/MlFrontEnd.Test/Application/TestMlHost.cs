﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHost.Application;
using MlHost.Models;
using MlHostSdk.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlFrontEnd.Test.Application
{
    internal class TestMlHost
    {
        protected IHost? _host;
        protected HttpClient? _client;

        public TestMlHost() { }

        public HttpClient HttpClient => _client ?? throw new ArgumentNullException(nameof(HttpClient));

        public T Resolve<T>() where T : class => _host?.Services.GetService<T>() ?? throw new InvalidOperationException($"Cannot find service {typeof(T).Name}");

        public TestMlHost StartApiServer()
        {
            IOption option = GetOption();

            var host = new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<MlHost.Startup>();
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

        public async Task WaitForStartup()
        {
            var token = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            while (!token.IsCancellationRequested)
            {
                var response = await HttpClient.GetAsync("api/ping");
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                PingResponse result = Resolve<IJson>().Deserialize<PingResponse>(responseString);

                switch (result.Status)
                {
                    case string v when v == ExecutionState.Running.ToString():
                        return;

                    case string v when v == ExecutionState.Failed.ToString():
                        throw new InvalidOperationException("Service failed to start");
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            throw new TimeoutException("Server has not started");
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

        private IOption GetOption()
        {
            string packageFile = FileTools.WriteResourceToTempFile("TestModel.mlPackage", nameof(TestMlHost), typeof(TestMlHost), "MlFrontEnd.Test.Package.TestModel.mlPackage");

            string[] args = new[]
            {
                "ServiceUri=http://localhost:5003/predict",
                $"PackageFile={packageFile}",
                "Port=5010",
            };

            return new OptionBuilder()
                .SetArgs(args)
                .Build();
        }
    }
}