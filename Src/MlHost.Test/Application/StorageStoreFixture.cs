using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MlHost.Application;
using MlHost.Services.PackageSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlHost.Test.Application
{
    public class StorageStoreFixture : TestWebsiteHost, IDisposable
    {
        public StorageStoreFixture()
        {
            string[] args = new[]
            {
                "ServiceUri=http://localhost:5003/predict",
                "ForceDeployment=true",
                "ZipFileUri=squad/ml-test-package.zip",
                "BlobStore:ContainerName=mldatamodels",
                "Deployment:DeploymentFolder=testDeploymentFolder",
                "Deployment:PackageFolder=testPackageFolder",
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
    }
}
