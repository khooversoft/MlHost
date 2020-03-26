using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MlHost.Services;
using MlHostApi.Models;
using MlHostApi.Services;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Test.Application
{
    public abstract class TestWebsiteHost : IDisposable
    {
        protected IHost? _host;
        protected HttpClient? _client;

        public HttpClient Client => _client ?? throw new ArgumentNullException(nameof(Client));

        public T Resolve<T>() where T : class => _host?.Services.GetService<T>() ?? throw new InvalidOperationException($"Cannot find service {typeof(T).Name}");

        public async Task WaitForStartup()
        {
            var token = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            while (!token.IsCancellationRequested)
            {
                var response = await Client.GetAsync("api/ping");
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                PingResponse result = Resolve<IJson>().Deserialize<PingResponse>(responseString);

                if (result.Status == ExecutionState.Running.ToString()) return;

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
    }
}
