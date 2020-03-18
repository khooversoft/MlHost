using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using System.Runtime.CompilerServices;

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
                    builder
                        .AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("", LogLevel.Trace);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
