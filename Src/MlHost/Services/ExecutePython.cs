using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Tools;
using System;
using System.Diagnostics;
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

        public ExecutePython(ILogger<ExecutePython> logger, IOption option, IExecutionContext executionContext)
        {
            _logger = logger;
            _option = option;
            _executionContext = executionContext;
        }

        public Task Run()
        {
            var timeout = TimeSpan.FromMinutes(5);

            KillAnyRunningProcesses();

            string fullPath = Path.Combine(_option.Deployment!.DeploymentFolder, @"python-3.8.1.amd64\python.exe");
            if (!File.Exists(fullPath)) throw new FileNotFoundException(fullPath);

            var tcs = new TaskCompletionSource<bool>();
            var scopedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(timeout).Token, _executionContext.TokenSource.Token);
            scopedTokenSource.Token.Register(() =>
            {
                _logger.LogError($"Python failed to start within timeout of {timeout}");
                tcs.SetException(new TimeoutException("Python child process failed to start"));
            });

            _logger.LogInformation("Starting python child process");

            var localProcess = new LocalProcess(_logger)
            {
                File = fullPath,
                Arguments = @".\app.py",
                WorkingDirectory = _option.Deployment.DeploymentFolder,
                CaptureOutput = WaitForRunning,
            };

            Task completeTask = localProcess.Run(_executionContext.TokenSource.Token);

            _logger.LogInformation("Python process is starting up");

            return Task.WhenAll(completeTask, tcs.Task);

            // ====================================================================================
            // Output function to test for "running" condition of Python
            bool WaitForRunning(string subject)
            {
                const string lookFor = "Running on";

                if (subject.IndexOf(lookFor) >= 0)
                {
                    _logger.LogInformation("Python child process is running");
                    _executionContext.State = ExecutionState.Running;

                    tcs.SetResult(true);
                    scopedTokenSource.Dispose();
                    return false;
                }

                return true;
            }
        }

        public Task<bool> KillAnyRunningProcesses()
        {
            foreach(var process in Process.GetProcessesByName("python"))
            {
                _logger.LogWarning($"Killing already running python.exe process {process.ProcessName} before starting child process");

                try 
                { 
                    process.Kill(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Cannot kill process {process.ProcessName}");
                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(true);
        }
    }
}
