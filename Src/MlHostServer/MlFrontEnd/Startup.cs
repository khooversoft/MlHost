using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlFrontEnd.Application;
using MlFrontEnd.Services;
using NSwag;
using Polly;
using Polly.Extensions.Http;
using Toolbox.Application;
using Toolbox.Services;

namespace MlHostFrontEnd
{
    public class Startup
    {
        const string _policyName = "defaultPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<BatchService>();
            services.AddSingleton<HostProxyService>();
            services.AddSingleton<IJson, Json>();

            // Create HttpClient factory for each version Id
            using ServiceProvider service = services.BuildServiceProvider();
            IOption option = service.GetRequiredService<IOption>();

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Title = "ML Front End";
                    document.Info.Description = "Front end for accessing ML models";
                    document.Schemes = new[] { OpenApiSchema.Http | OpenApiSchema.Https };
                };
            });

            foreach (var item in option.Hosts)
            {
                //services
                //    .AddHttpClient(item.ModelName, httpClient => httpClient.BaseAddress = new Uri(item.Uri));

                services
                    .AddHttpClient(item.ModelName, httpClient => httpClient.BaseAddress = new Uri(item.Uri))
                        .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                        .AddPolicyHandler(GetRetryPolicy());
            }

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOption option)
        {
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

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() => HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
