using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MlHost.Application;
using MlHost.Services;
using MlHost.Tools;
using MlHostApi.Repository;
using Toolbox.Repository;
using Toolbox.Services;

namespace MlHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IQuestion, QuestionService>();
            services.AddSingleton<IExecutionContext, ExecutionContext>();
            services.AddSingleton<IPackageDeployment, PackageDeployment>();
            services.AddSingleton<IExecutePython, ExecutePython>();
            services.AddSingleton<IJson, Json>();
            services.AddSingleton<IPackageSource, PackageSourceFromStorage>();
            services.AddSingleton<IModelRepository, ModelRepository>();

            services.AddSingleton<IDatalakeRepository>(x =>
            {
                IOption option = x.Resolve<IOption>();
                return new DatalakeRepository(option.Store!);
            });

            services.AddHostedService<PythonHostedService>();
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
