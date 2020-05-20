using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using MlProcess.Activities;
using MlProcess.Application;
using MlProcess.Models;
using MlProcess.Services;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Logging;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlProcess
{
    class Program
    {
        private const int _ok = 0;
        private const int _error = 1;
        private const string _lifetimeScopeTag = "main";
        private readonly string _programTitle = $"ML Process - Version {Assembly.GetExecutingAssembly().GetName().Version}";

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
                    .Append(string.Empty)
                    .ForEach(x => Console.WriteLine(x));

                return _ok;
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var execContext = new ExecContext(cancellationTokenSource.Token, option);

            using (ILifetimeScope container = CreateContainer(option).BeginLifetimeScope(_lifetimeScopeTag))
            {
                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
                {
                    e.Cancel = true;
                    cancellationTokenSource.Cancel();
                    Console.WriteLine("Canceling...");
                };

                option.DumpConfigurations(container.Resolve<ILogger<Program>>());

                var activities = new Func<Task>[]
                {
                    () => option.Run ? container.Resolve<BuildJournalFile>().Build(execContext) : Task.CompletedTask,
                    () => option.Run ? container.Resolve<RunModels>().Run(execContext) : Task.CompletedTask,
                    () => option.Run ? container.Resolve<ReportMetricsResult>().Report() : Task.CompletedTask,
                    () => option.Run || option.Append ? container.Resolve<AppendResults>().Build(execContext) : Task.CompletedTask,
                };

                await activities
                    .Where(_ => !cancellationTokenSource.Token.IsCancellationRequested)
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
            builder.RegisterType<HttpRest>().As<IHttpRest>().InstancePerLifetimeScope();
            builder.RegisterType<MetricSampler>().As<IMetricSampler>().InstancePerLifetimeScope();

            builder.RegisterType<BuildJournalFile>();
            builder.RegisterType<RunModels>();
            builder.RegisterType<AppendResults>();
            builder.RegisterType<ReportMetricsResult>();
            builder.RegisterType<RequestJournalWriter>();
            builder.RegisterType<RequestJournalReader>();
            builder.RegisterType<ResponseJournalWriter>();
            builder.RegisterType<ResponseJournalReader>();

            // Logging
            var loggerFactory = LoggerFactory.Create(configure =>
            {
                configure.AddConsole(x => x.Format = ConsoleLoggerFormat.Systemd);

                if (option.LogFile.ToNullIfEmpty() != null)
                {
                    configure.AddFile(Path.GetDirectoryName(option.LogFile)!, Path.GetFileNameWithoutExtension(option.LogFile)!);
                }
            });

            builder.RegisterInstance(loggerFactory).As<ILoggerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).InstancePerDependency();

            return builder.Build();
        }
    }
}
