using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Services;
using System.IO;
using System.Runtime.CompilerServices;
using Toolbox.Logging;
using Toolbox.Services;
using Toolbox.Tools;

[assembly: InternalsVisibleTo("MlHost.Test")]

namespace MlHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IOption option = new OptionBuilder()
                .AddCommandLine(args)
                .AddJsonFile("appsettings.json")
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
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("", LogLevel.Trace);

                    if (option.LogFile.ToNullIfEmpty() != null)
                    {
                        builder.AddFile(Path.GetDirectoryName(option.LogFile)!, Path.GetFileNameWithoutExtension(option.LogFile)!);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
