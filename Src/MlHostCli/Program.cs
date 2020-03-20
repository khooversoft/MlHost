using Autofac;
using MlHostApi.Repository;
using MlHostApi.Tools;
using MlHostCli.Activity;
using MlHostCli.Application;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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
                .AddCommandLine(args)
                .Build();

            if (option.Help)
            {
                option.GetHelp()
                    .ForEach(x => Console.WriteLine(x));

                return _ok;
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            using (ILifetimeScope container = CreateContainer(option).BeginLifetimeScope(_lifetimeScopeTag))
            {
                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
                {
                    e.Cancel = true;
                    cancellationTokenSource.Cancel();
                    Console.WriteLine("Canceling...");
                };

                var activities = new Func<Task?>[]
                {
                    () => option.List ? container.Resolve<ListModelActivity>().List(cancellationTokenSource.Token) : null,
                    () => option.Upload ? container.Resolve<UploadModelActivity>().Upload(cancellationTokenSource.Token) : null,
                    () => option.Download ? container.Resolve<DownloadModelActivity>().Download(cancellationTokenSource.Token) : null,
                    () => option.Delete ? container.Resolve<DeleteModelActivity>().Delete(cancellationTokenSource.Token) : null,
                    () => option.Activate ? container.Resolve<ActivateModelActivity>().Activate(cancellationTokenSource.Token) : null,
                };

                await activities
                    .Where(x => x != null)
                    .ForEachAsync(async x => await x()!);

                Console.WriteLine("Completed");
                return _ok;
            }
        }

        private IContainer CreateContainer(IOption option)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(option).As<IOption>();

            if (option.BlobStore != null)
            {
                builder.RegisterInstance(new BlobRepository(option.BlobStore.ContainerName, option.BlobStore.ConnectionString)).As<IBlobRepository>().InstancePerLifetimeScope();
                builder.RegisterType<ModelRepository>().As<IModelRepository>().InstancePerLifetimeScope();
                builder.RegisterType<ActivateModelActivity>();
                builder.RegisterType<DeleteModelActivity>();
                builder.RegisterType<DownloadModelActivity>();
                builder.RegisterType<ListModelActivity>();
                builder.RegisterType<UploadModelActivity>();
            }

            return builder.Build();
        }
    }
}
