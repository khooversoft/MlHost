using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Models;
using MlHostSdk.Package;
using MlHostSdk.Types;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Tools;
using Toolbox.Services;

namespace MlHost.Services
{
    /// <summary>
    /// Execute model background service
    /// </summary>
    internal class ExecuteModel : IExecuteModel
    {
        private readonly ILogger<ExecuteModel> _logger;
        private readonly IOption _option;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IExecutionContext _executionContext;
        private SubjectScope<LocalProcess>? _localProcess;

        public ExecuteModel(IOption option, ILoggerFactory loggerFactory, IExecutionContext executionContext)
        {
            _logger = loggerFactory.CreateLogger<ExecuteModel>();
            _option = option;
            _loggerFactory = loggerFactory;
            _executionContext = executionContext;
        }

        public Task Start()
        {
            _executionContext.DeploymentFolder.VerifyNotEmpty(nameof(_executionContext.DeploymentFolder));

            _logger.LogInformation($"Starting ML child process, deployment folder={_executionContext.DeploymentFolder}");

            var tcs = new TaskCompletionSource<bool>();

            ModelExecuteDetails modelExecute =
                ReadManifest()
                ?? CheckIfRunExist()
                ?? throw new InvalidOperationException("No support for starting model");

            _localProcess = new LocalProcessBuilder()
            {
                ExecuteFile = modelExecute.ExecutePath,
                Arguments = modelExecute.Arguments,
                WorkingDirectory = _executionContext.DeploymentFolder,
                CaptureOutput = lineData =>
                {
                    switch (lineData)
                    {
                        case string subject when LookFor(subject, modelExecute.StartSignal):
                            _logger.LogInformation($"Detected running command: {lineData}");
                            tcs.SetResult(true);
                            _executionContext.State = ExecutionState.Running;
                            break;

                        case string subject when LookFor(subject, "RuntimeError", "not enough memory"):
                            _logger.LogError($"Detected running runtime error, not enough memory, {lineData}");
                            _executionContext.State = ExecutionState.Failed;
                            break;
                    }
                }
            }
            .Build(_loggerFactory.CreateLogger<LocalProcess>())
            .ToSubjectScope();

            _localProcess.Subject.Run();

            _logger.LogInformation("ML process is starting up");
            return tcs.Task;
        }

        public void Stop()
        {
            _logger.LogInformation("Stopping ML process");

            _localProcess?.GetAndClear()?.Stop();
        }

        private bool LookFor(string lineData, params string[] searchFor) => searchFor
            .VerifyAssert(x => x.Length > 0, $"Invalid {nameof(searchFor)}, must have at least 1 element")
            .All(x => (lineData ?? string.Empty).IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0);

        private ModelExecuteDetails? ReadManifest()
        {
            MlPackageManifestFile manifestFile = new MlPackageManifestFile(Path.Combine(_executionContext.DeploymentFolder!, MlPackageBuilder.ManifestFileName));

            return new ModelExecuteDetails
            {
                ExecutePath = _option.PropertyResolver.Resolve(manifestFile.RunCmd),
                Arguments = _option.PropertyResolver.Resolve(manifestFile.Arguments),
                StartSignal = _option.PropertyResolver.Resolve(manifestFile.MlPackageManifest.StartSignal),
            };
        }

        private ModelExecuteDetails? CheckIfRunExist()
        {
            string fullPath = Path.Combine(_executionContext.DeploymentFolder!, "run.ps1")
                .VerifyAssert<string, FileNotFoundException>(x => File.Exists(x), x => x);

            return new ModelExecuteDetails
            {
                ExecutePath = "powershell.exe",
                Arguments = $"-File {_option.PropertyResolver.Resolve(fullPath)}",
            };
        }

        private class ModelExecuteDetails
        {
            public string ExecutePath { get; set; } = null!;

            public string? Arguments { get; set; }

            public string StartSignal { get; set; } = "Running on";
        }
    }
}
