using Microsoft.CodeAnalysis.CSharp.Syntax;
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

            _logger.LogInformation($"Starting python child process, deployment folder={_executionContext.DeploymentFolder}");

            var tcs = new TaskCompletionSource<bool>();
            ModelExecute modelExecute = GetExecutableAndCommand();

            _localProcess = new LocalProcessBuilder()
            {
                ExecuteFile = modelExecute.ExecutePath,
                Arguments = modelExecute.Arguments,
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

        private ModelExecute GetExecutableAndCommand()
        {
            ModelConfiguration modelConfiguration = GetModelConfiguration();
            return GetExecute(modelConfiguration) ?? Python(modelConfiguration);
        }

        private ModelConfiguration GetModelConfiguration()
        {
            string metadataFile = Path.Combine(_executionContext.DeploymentFolder!, "metadata.json")
                .VerifyAssert<string, FileNotFoundException>(x => File.Exists(x), x => x);

            string json = File.ReadAllText(metadataFile);
            return _json.Deserialize<ModelConfiguration>(json);
        }

        private ModelExecute? GetExecute(ModelConfiguration modelConfiguration)
        {
            if (string.IsNullOrWhiteSpace(modelConfiguration.Execute)) return null;

            int spaceIndex = modelConfiguration.Execute.IndexOf(' ');
            if (spaceIndex < 0) return new ModelExecute(modelConfiguration.Execute, null);

            string executePath = modelConfiguration.Execute.Substring(0, spaceIndex).Trim();
            string arguments = modelConfiguration.Execute.Substring(spaceIndex + 1).Trim();

            executePath = Path.Combine(_executionContext.DeploymentFolder!, executePath);
            return new ModelExecute(executePath, arguments);
        }

        private ModelExecute Python(ModelConfiguration modelConfiguration)
        {
            const string pythonText = "python";

            string executableFolder = modelConfiguration.Python
                ?.VerifyAssert(x => x.ToNullIfEmpty() != null, $"{pythonText} is empty")!;

            string executablePath = Path.Combine(_executionContext.DeploymentFolder!, executableFolder, "python.exe")
                .VerifyAssert<string, FileNotFoundException>(x => File.Exists(x), x => x);

            return new ModelExecute(executableFolder, "-m langserve");
        }

        private class ModelConfiguration
        {
            public string Python { get; set; } = null!;
            public string? Execute { get; set; }
        }

        private struct ModelExecute
        {
            public ModelExecute(string executePath, string? arguments)
            {
                ExecutePath = executePath;
                Arguments = arguments;
            }

            public string ExecutePath { get; }
            public string? Arguments { get; }
        }
    }
}
