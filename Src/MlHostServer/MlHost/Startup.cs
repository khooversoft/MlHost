using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlHost.Models;
using MlHost.Services;
using MlHost.Tools;
using NSwag;
using System;
using Toolbox.Application;
using Toolbox.Services;

namespace MlHost
{
    public class Startup
    {
        const string _policyName = "defaultPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        internal IConfiguration Configuration { get; }

        internal ITelemetryMemory TelemetryMemory { get; } = new TelemetryMemory();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(TelemetryMemory);
            services.AddControllers();
            services.AddSingleton<IPredictService, PredictService>();
            services.AddSingleton<IExecutionContext, ExecutionContext>();
            services.AddSingleton<IExecuteModel, ExecuteModel>();
            services.AddSingleton<IJson, Json>();
            services.AddSingleton<IDeployPackage, DeployPackage>();
            services.AddSingleton<ICleanupProcess, CleanupProcess>();

            services.AddHostedService<MlHostedService>();

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Title = "ML Host";
                    document.Info.Description = "Host for Machine Learning Models";
                    document.Schemes = new[] { OpenApiSchema.Http | OpenApiSchema.Https };
                };
            });

            services.AddCors(x => x.AddPolicy(_policyName, builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetPreflightMaxAge(TimeSpan.FromHours(1));
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, Application.IOption option)
        {
            loggerFactory.AddProvider(new TelemetryMemoryLoggerProvider(TelemetryMemory));

            if (env.IsDevelopment() || option.RunEnvironment == RunEnvironment.Dev)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseCors(_policyName);
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
