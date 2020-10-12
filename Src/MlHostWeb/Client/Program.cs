using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MlHostWeb.Client.Services;
using Toolbox.Services;
using Radzen;

namespace MlHostWeb.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddScoped<NavMenuService>();
            builder.Services.AddSingleton<ClientContentService>();
            builder.Services.AddSingleton<HostConfigurationService>();
            builder.Services.AddSingleton<StateCacheService>();
            builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();

            builder.RootComponents.Add<App>("app");

            var host = builder.Build();

            HostConfigurationService modelConfiguration = host.Services.GetRequiredService<HostConfigurationService>();
            await modelConfiguration.Initialize();

            await host.RunAsync();
        }
    }
}
