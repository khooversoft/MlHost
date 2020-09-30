using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using MlHost.Application;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Toolbox.Application;
using Toolbox.Logging;
using Toolbox.Tools;

[assembly: InternalsVisibleTo("MlHost.Test")]
[assembly: InternalsVisibleTo("MlFrontEnd.Test")]

namespace MlHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(new Option().HostVersionTitle());
            Console.WriteLine();

            IOption option = new OptionBuilder()
                .SetArgs(args)
                .SetJsonFile("appsettings.json")
                .Build();

            CreateHostBuilder(args, option)
                .Build()
                .Run();
        }

        internal static IHostBuilder CreateHostBuilder(string[] args, IOption option) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(option);

                    if (option.RunEnvironment != RunEnvironment.Dev)
                    {
                        services.AddApplicationInsightsTelemetry();
                    }
                })
                .ConfigureLogging(builder =>
                {
                    if (option.RunEnvironment != RunEnvironment.Dev)
                    {
                        builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("", LogLevel.Trace);
                    }

                    if (option.LogFile.ToNullIfEmpty() != null)
                    {
                        builder.AddFile(Path.GetDirectoryName(option.LogFile)!, Path.GetFileNameWithoutExtension(option.LogFile)!);
                    }

                    builder.AddDebug();
                    builder.AddFilter<DebugLoggerProvider>(x => true);
                    builder.AddFilter(x => true);

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    if (option.RunEnvironment == RunEnvironment.Dev)
                    {
                        webBuilder.UseUrls($"http://localhost:{option.Port}");
                    }
                });
    }
}
