using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlFrontEnd.Application;
using Toolbox.Application;

[assembly: InternalsVisibleTo("MlHost.Test")]
[assembly: InternalsVisibleTo("MlFrontEnd.Test")]

namespace MlHostFrontEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IOption option = new OptionBuilder()
                .SetArgs(args)
                .Build();

            CreateHostBuilder(args, option)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IOption option) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IOption>(option);
                })
                .ConfigureLogging(config =>
                {
                    if( option.RunEnvironment == RunEnvironment.Dev)
                    {
                        config
                            .AddConsole()
                            .AddDebug()
                            .AddFilter(x => true);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    if (option.ApplicationUrl != null) webBuilder.UseUrls(option.ApplicationUrl);
                });
    }
}
