using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MlHost.Application;
using System;

namespace MlHost.Test.Application
{
    public class TestHostWithStorage : TestWebsiteHost, IDisposable
    {
        private static TestHostWithStorage? _currentHost;
        private static readonly object _lock = new object();

        public TestHostWithStorage()
        {
            string[] args = new[]
            {
                "ServiceUri=http://localhost:5003/predict",
                //"ForceDeployment=true",

                "BlobStore:ContainerName=model-test",
                "BlobStore:AccountName=mlteststoragev1",

                "HostName=mlhost001",
            };

            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .AddUserSecrets("MlHost.Test")
                .Build();

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

        public static TestHostWithStorage GetHost()
        {
            lock (_lock)
            {
                return _currentHost ??= new TestHostWithStorage();
            }
        }
    }
}
