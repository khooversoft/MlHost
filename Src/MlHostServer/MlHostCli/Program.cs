using Autofac;
using Microsoft.Extensions.Logging;
using MlHostSdk.Repository;
using MlHostCli.Activity;
using MlHostCli.Application;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Repository;
using Toolbox.Services;
using Toolbox.Tools;

[assembly: InternalsVisibleTo("MlHostCli.Test")]

namespace MlHostCli
{
    class Program
    {
        private const int _ok = 0;
        private const int _error = 1;
        private const string _lifetimeScopeTag = "main";
        private readonly string _programTitle = $"ML Host Command Line Interface - Version {Assembly.GetExecutingAssembly().GetName().Version}";

        static async Task<int> Main(string[] args)
        {
            try
            {
                return await new Program().Run(args);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                DisplayStartDetails(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhanded exception: " + ex.ToString());
                DisplayStartDetails(args);
            }

            return _error;
        }

        private static void DisplayStartDetails(string[] args) => Console.WriteLine($"Arguments: {string.Join(", ", args)}");

        private async Task<int> Run(string[] args)
        {
            Console.WriteLine(_programTitle);
            Console.WriteLine();

            IOption option = new OptionBuilder()
                .SetArgs(args)
                .Build();

            if (option.Help)
            {
                option.GetHelp()
                    .Append(string.Empty)
                    .ForEach(x => Console.WriteLine(x));

                return _ok;
            }

            option.DumpConfigurations();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            using (ILifetimeScope container = CreateContainer(option).BeginLifetimeScope(_lifetimeScopeTag))
            {
                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
                {
                    e.Cancel = true;
                    cancellationTokenSource.Cancel();
                    Console.WriteLine("Canceling...");
                };

                var activities = new Func<Task>[]
                {
                    () => option.Upload ? container.Resolve<UploadModelActivity>().Upload(cancellationTokenSource.Token) : Task.CompletedTask,
                    () => option.Download ? container.Resolve<DownloadModelActivity>().Download(cancellationTokenSource.Token) : Task.CompletedTask,
                    () => option.Delete ? container.Resolve<DeleteModelActivity>().Delete(cancellationTokenSource.Token) : Task.CompletedTask,
                    () => option.Bind ? container.Resolve<BindActivity>().Bind(cancellationTokenSource.Token) : Task.CompletedTask,
                    () => option.Swagger ? container.Resolve<BuildSwaggerActivity>().Write(cancellationTokenSource.Token) : Task.CompletedTask,
                    () => option.Build ? container.Resolve<BuildActivity>().Build(cancellationTokenSource.Token) : Task.CompletedTask,
                };

                await activities
                    .ForEachAsync(async x => await x());
            }

            Console.WriteLine();
            Console.WriteLine("Completed");
            return _ok;
        }

        private IContainer CreateContainer(IOption option)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(option).As<IOption>();
            builder.RegisterInstance(option.SecretFilter ?? new SecretFilter()).As<ISecretFilter>();

            builder.RegisterType<Json>().As<IJson>().InstancePerLifetimeScope();
            builder.RegisterType<BuildSwaggerActivity>();
            builder.RegisterType<BuildActivity>();

            // Logging
            var loggerFactory = LoggerFactory.Create(configure => configure.AddConsole());
            builder.RegisterInstance(loggerFactory).As<ILoggerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).InstancePerDependency();
            
            if (option.Store != null)
            {
                builder.RegisterInstance(new DatalakeStore(option.Store)).As<IDatalakeStore>();
                builder.RegisterType<DatalakeModelStore>().As<IModelStore>().InstancePerLifetimeScope();

                builder.RegisterType<DeleteModelActivity>();
                builder.RegisterType<DownloadModelActivity>();
                builder.RegisterType<UploadModelActivity>();
                builder.RegisterType<BindActivity>();
            }

            return builder.Build();
        }
    }
}
