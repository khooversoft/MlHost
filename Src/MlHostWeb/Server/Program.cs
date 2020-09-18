using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using MlHostWeb.Server.Application;
using Toolbox.Application;

namespace MlHostWeb.Server
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

        internal static IHostBuilder CreateHostBuilder(string[] args, IOption option) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(option);
                })
                .ConfigureLogging(builder =>
                {
                    if (option.Environment.ConvertToEnvironment() == RunEnvironment.Dev)
                    {
                        builder.AddDebug();
                        builder.AddFilter<DebugLoggerProvider>(x => true);
                        builder.AddFilter(x => true);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
