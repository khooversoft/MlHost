using Microsoft.Extensions.Logging;
using MlHost.Tools;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Services
{
    /// <summary>
    /// Execute pyton 
    /// </summary>
    public class ExecutePython : IExecutePython
    {
        private readonly ILogger<ExecutePython> _logger;
        private readonly IExecutionContext _executionContext;

        public ExecutePython(ILogger<ExecutePython> logger, IExecutionContext executionContext)
        {
            _logger = logger;
            _executionContext = executionContext;
        }

        public Task Run(string deploymentFolder)
        {
            var timeout = TimeSpan.FromMinutes(5);
            if (string.IsNullOrWhiteSpace(deploymentFolder)) throw new ArgumentException(nameof(deploymentFolder));

            KillAnyRunningProcesses();

            string fullPath = Path.Combine(deploymentFolder, @"python-3.8.1.amd64\python.exe");
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
                WorkingDirectory = deploymentFolder,
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
                    _executionContext.Running = true;

                    tcs.SetResult(true);
                    scopedTokenSource.Dispose();
                    return false;
                }

                return true;
            }
        }

        private void KillAnyRunningProcesses()
        {
            foreach(var process in Process.GetProcessesByName("python.exe"))
            {
                _logger.LogWarning($"Killing already running python.exe process '{process.ProcessName} before starting child process");
                try { process.Kill(true); } catch { }
            }
        }
    }
}
