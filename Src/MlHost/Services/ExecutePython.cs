using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Tools;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Services;
using Toolbox.Tools;
using Toolbox.Tools.Local;

namespace MlHost.Services
{
    /// <summary>
    /// Execute python 
    /// </summary>
    internal class ExecutePython : IExecutePython
    {
        private readonly ILogger<ExecutePython> _logger;
        private readonly IOption _option;
        private readonly IExecutionContext _executionContext;
        private SubjectScope<MonitorLocalProcess>? _monitorLocalProcess;
        private SubjectScope<CancellationTokenSource>? _token;

        public ExecutePython(ILogger<ExecutePython> logger, IOption option, IExecutionContext executionContext)
        {
            _logger = logger;
            _option = option;
            _executionContext = executionContext;
        }

        public Task Start()
        {
            _executionContext.DeploymentFolder.VerifyNotEmpty(nameof(_executionContext.DeploymentFolder));

            string fullPath = Path.Combine(_executionContext.DeploymentFolder!, "run.ps1")
                .VerifyAssert<string, FileNotFoundException>(x => File.Exists(x), x => x);

            _logger.LogInformation($"Starting python child process, deployment folder={_executionContext.DeploymentFolder}");

            RegisterTimeoutAndCancelation();
            var tcs = new TaskCompletionSource<bool>();

            _monitorLocalProcess = new LocalProcessBuilder()
            {
                ExecuteFile = "powershell.exe",
                Arguments = $"-File {fullPath}",
                WorkingDirectory = _executionContext.DeploymentFolder,
            }
            .Build(lineData =>
            {
                switch (lineData)
                {
                    case string subject when LookFor(subject, "Running on"):
                        _logger.LogInformation($"Detected running command: {lineData}");
                        tcs.SetResult(true);
                        _executionContext.State = ExecutionState.Running;
                        return MonitorState.Running;

                    case string subject when LookFor(subject, "RuntimeError", "not enough memory"):
                        _logger.LogError($"Detected running runtime error, not enough memory, {lineData}");
                        _executionContext.State = ExecutionState.Restarting;
                        return MonitorState.Restart;

                    default:
                        return null;
                }
            }, _logger)
            .ToSubjectScope();

            _monitorLocalProcess.Subject.Start(_executionContext.TokenSource.Token);

            _logger.LogInformation("Python process is starting up");
            return tcs.Task;
        }

        public void Stop()
        {
            _logger.LogInformation("Stopping ML process");

            _monitorLocalProcess?.GetAndClear()?.Stop();
        }

        private void RegisterTimeoutAndCancelation()
        {
            _token = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token, _executionContext.TokenSource.Token)
                .ToSubjectScope();

            _token.Subject.Token.Register(() =>
            {
                _logger.LogInformation($"{nameof(ExecutePython)} canceled");
                Stop();
            });
        }

        private bool LookFor(string lineData, params string[] searchFor) => searchFor
            .VerifyAssert(x => x.Length > 0, $"Invalid {nameof(searchFor)}, must have at least 1 element")
            .All(x => (lineData ?? string.Empty).IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0);
    }
}
