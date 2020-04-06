using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MlHost.Application;
using MlHost.Services;
using MlHostApi.Models;
using MlHostApi.Repository;
using MlHostApi.Types;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Repository;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHost.Test.Application
{
    internal class TestWebsiteHost : IDisposable
    {
        protected IHost? _host;
        protected HttpClient? _client;
        private static TestWebsiteHost? _currentHost;
        private static SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public TestWebsiteHost() { }

        public HttpClient Client => _client ?? throw new ArgumentNullException(nameof(Client));

        public T Resolve<T>() where T : class => _host?.Services.GetService<T>() ?? throw new InvalidOperationException($"Cannot find service {typeof(T).Name}");

        public static async Task<TestWebsiteHost> GetHost()
        {
            await _lock.WaitAsync();

            try
            {
                if (_currentHost != null) return _currentHost;

                _currentHost = new TestWebsiteHost();
                IOption option = _currentHost.GetOption();

                await _currentHost.UploadMlTestPackage(option);
                _currentHost.StartApiServer(option);

                return _currentHost;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task WaitForStartup()
        {
            var token = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            while (!token.IsCancellationRequested)
            {
                var response = await Client.GetAsync("api/ping");
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                PingResponse result = Resolve<IJson>().Deserialize<PingResponse>(responseString);

                switch (result.Status)
                {
                    case string v when v == ExecutionState.Running.ToString():
                        return;

                    case string v1 when v1 == ExecutionState.Failed.ToString():
                    case string v2 when v2 == ExecutionState.NoModelRegisteredForHost.ToString():
                        throw new InvalidOperationException("Service failed to start");
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            throw new TimeoutException("Server has not started");
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

        private void StartApiServer(IOption option)
        {
            var host = new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(option);
                });

            _host = host.Start();
            _client = _host.GetTestServer().CreateClient();
        }

        private IOption GetOption()
        {
            string[] args = new[]
            {
                "ServiceUri=http://localhost:5003/predict",
                "ForceDeployment=true",

                "Store:ContainerName=model-test",
                "Store:AccountName=mltestadlsv1",

                "HostName=mlhost001",
            };

            return new OptionBuilder()
                .AddCommandLine(args)
                .AddUserSecrets("MlHost.Test")
                .Build();
        }

        private async Task UploadMlTestPackage(IOption option)
        {
            ModelId modelId = new ModelId("test/TestModel");
            IDatalakeRepository datalakeRepository = new DatalakeRepository(option.Store!);
            IModelRepository modelRepository = new ModelRepository(datalakeRepository, new Json());

            string downloadFile = FileTools.WriteResourceToTempFile("TestModel.mlPackage", typeof(TestWebsiteHost), "MlHost.Test.Package.TestModel.mlPackage");
            await modelRepository.Upload(downloadFile, modelId, true, CancellationToken.None);
            await modelRepository.AddActivation(option.HostName!, modelId, CancellationToken.None);
        }
    }
}
