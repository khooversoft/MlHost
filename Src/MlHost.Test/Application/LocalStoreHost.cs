//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.TestHost;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using MlHost.Application;
//using MlHost.Services.PackageSource;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace MlHost.Test.Application
//{
//    public class LocalStoreFixture : TestWebsiteHost, IDisposable
//    {
//        public LocalStoreFixture()
//        {
//            string[] args = new[]
//            {
//                "ServiceUri=http://localhost:5003/predict",
//                "ForceDeployment=true",
//                "ZipFileUri=notValid",
//                "BlobStore:ContainerName=notValid",
//                "BlobStore:ConnectionString=notValid",
//                "Deployment:DeploymentFolder=testDeploymentFolder",
//                "Deployment:PackageFolder=notValid",
//            };

//            IOption option = new OptionBuilder()
//                .AddCommandLine(args)
//                .Build();

//            var host = new HostBuilder()
//                .ConfigureWebHostDefaults(webBuilder =>
//                {
//                    webBuilder
//                        .UseTestServer()
//                        .UseStartup<Startup>();
//                })
//                .ConfigureServices((hostContext, services) =>
//                {
//                    services
//                        .AddSingleton(option)
//                        .AddSingleton<IPackageSource>(new PackageSourceFromResource(typeof(LocalStoreFixture), "MlHost.Test.Package.ml-test-package.zip"));
//                });

//            _host = host.Start();
//            _client = _host.GetTestServer().CreateClient();
//        }
//    }
//}
