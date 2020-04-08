using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlHost.Services;
using MlHost.Tools;
using Toolbox.Services;

namespace MlHost
{
    public class Startup
    {
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
            services.AddSingleton<IQuestion, QuestionService>();
            services.AddSingleton<IExecutionContext, ExecutionContext>();
            services.AddSingleton<IExecutePython, ExecutePython>();
            services.AddSingleton<IJson, Json>();

            services.AddHostedService<PythonHostedService>();
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new TelemetryMemoryLoggerProvider(TelemetryMemory));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
