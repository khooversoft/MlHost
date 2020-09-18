using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Services;
using Toolbox.Tools;
//using Toolbox.Tools.Local;

namespace MlHost.Services
{
    /// <summary>
    /// Execute python 
    /// </summary>
    internal class ExecutePython : IExecutePython
    {
        private readonly ILogger<ExecutePython> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IJson _json;
        private readonly IOption _option;
        private readonly IExecutionContext _executionContext;
        private SubjectScope<LocalProcess>? _localProcess;

        public ExecutePython(ILoggerFactory loggerFactory, IJson json, IOption option, IExecutionContext executionContext)
        {
            _logger = loggerFactory.CreateLogger<ExecutePython>();
            _loggerFactory = loggerFactory;
            _json = json;
            _option = option;
            _executionContext = executionContext;
        }

        public Task Start()
        {
            _executionContext.DeploymentFolder.VerifyNotEmpty(nameof(_executionContext.DeploymentFolder));

            string fullPath = GetExecutable();
            _logger.LogInformation($"Starting python child process, deployment folder={_executionContext.DeploymentFolder}");

            var tcs = new TaskCompletionSource<bool>();

            _localProcess = new LocalProcessBuilder()
            {
                ExecuteFile = fullPath,
                Arguments = "-m langserve",
                WorkingDirectory = _executionContext.DeploymentFolder,
                CaptureOutput = lineData =>
                {
                    switch (lineData)
                    {
                        case string subject when LookFor(subject, "Running on"):
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

            _logger.LogInformation("Python process is starting up");
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

        private string GetExecutable()
        {
            string metadataFile = Path.Combine(_executionContext.DeploymentFolder!, "metadata.json")
                .VerifyAssert<string, FileNotFoundException>(x => File.Exists(x), x => x);

            const string pythonText = "python";
            string json = File.ReadAllText(metadataFile);
            ModelConfiguration modelConfiguration = _json.Deserialize<ModelConfiguration>(json);

            string executableFolder = modelConfiguration.Python
                ?.VerifyAssert(x => x.ToNullIfEmpty() != null, $"{pythonText} is empty")!;

            string executableFilePath = Path.Combine(_executionContext.DeploymentFolder!, executableFolder, "python.exe")
                .VerifyAssert<string, FileNotFoundException>(x => File.Exists(x), x => x);

            return executableFilePath;
        }

        private class ModelConfiguration
        {
            public string? Python { get; set; }
        }
    }
}
