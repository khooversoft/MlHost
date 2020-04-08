using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Tools;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

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
        private readonly ITelemetryMemory _telemetryMemory;
        private Task? _localProcess;

        public ExecutePython(ILogger<ExecutePython> logger, IOption option, IExecutionContext executionContext, ITelemetryMemory telemetryMemory)
        {
            _logger = logger;
            _option = option;
            _executionContext = executionContext;
            _telemetryMemory = telemetryMemory;

            _telemetryMemory.Add($"([{nameof(ExecutePython)})] constructed");
        }

        public Task Run()
        {
            var timeout = TimeSpan.FromMinutes(5);

            string fullPath = Path.Combine(_option.DeploymentFolder, @"python-3.8.1.amd64\python.exe");
            if (!File.Exists(fullPath)) throw new FileNotFoundException(fullPath);

            var tcs = new TaskCompletionSource<bool>();
            var scopedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(timeout).Token, _executionContext.TokenSource.Token);
            scopedTokenSource.Token.Register(() =>
            {
                _logger.LogError($"Python failed to start within timeout of {timeout}");
                tcs.SetException(new TimeoutException("Python child process failed to start"));
            });

            _logger.LogInformation("Starting python child process");
            _telemetryMemory.Add($"([{nameof(ExecutePython)})] Starting python child process, deployment folder={_option.DeploymentFolder}");

            var localProcess = new LocalProcess(_logger)
            {
                File = fullPath,
                Arguments = @".\app.py",
                WorkingDirectory = _option.DeploymentFolder,
                CaptureOutput = WaitForRunning,
            };

            _localProcess = localProcess.Run(_executionContext.TokenSource.Token);

            _logger.LogInformation("Python process is starting up");

            return tcs.Task;

            // ====================================================================================
            // Output function to test for "running" condition of Python
            bool WaitForRunning(string subject)
            {
                const string lookFor = "Running on";

                if (subject.IndexOf(lookFor) >= 0)
                {
                    scopedTokenSource.Dispose();

                    _logger.LogInformation("Python child process is running");
                    _executionContext.State = ExecutionState.Running;
                    _telemetryMemory.Add($"([{nameof(ExecutePython)}] Python child process is running");

                    tcs.SetResult(true);
                    return false;
                }

                return true;
            }
        }

        public Task Stop() => _localProcess ?? Task.CompletedTask;
    }
}
