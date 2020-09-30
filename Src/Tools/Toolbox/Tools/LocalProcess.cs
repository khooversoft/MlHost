using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Tools
{
    public class LocalProcess
    {
        private readonly ILogger<LocalProcess> _logger;
        private Process? _process;

        public LocalProcess(LocalProcessBuilder builder, ILogger<LocalProcess> logger)
        {
            builder.VerifyNotNull(nameof(builder));

            _logger = logger.VerifyNotNull(nameof(logger));

            ExecuteFile = builder.ExecuteFile.VerifyNotEmpty(nameof(builder.ExecuteFile));
            Arguments = builder.Arguments;
            WorkingDirectory = builder.WorkingDirectory;
            CaptureOutput = builder.CaptureOutput;
        }

        public string? ExecuteFile { get; }

        public string? Arguments { get; }

        public string? WorkingDirectory { get; }

        public Action<string>? CaptureOutput { get; }

        public Task<Process> Run()
        {
            _process.VerifyAssert(x => x == null, "Process is running");

            _logger.LogTrace($"Starting local process, File={ExecuteFile}, Arguments={Arguments}, WorkingDirectory={WorkingDirectory}");

            TaskCompletionSource<Process> tcs = new TaskCompletionSource<Process>();

            Process savedProcess = _process = BuildProcess();
            _process.Exited += (data, e) =>
            {
                _logger.LogInformation($"Process has exit, ExitCode ={_process?.ExitCode}");
                tcs.SetResult(savedProcess);
            };

            // Start process
            bool started;
            try
            {
                started = _process.Start();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                _logger.LogError(ex, "Start failed");
                started = false;
            }

            if (!started)
            {
                string msg = $"Process failed to start, File={ExecuteFile}";
                _logger.LogError(msg);
                throw new InvalidOperationException(msg);
            }

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            return tcs.Task;
        }

        public void Stop()
        {
            Process? process = Interlocked.Exchange(ref _process, null!);
            if (process == null) return;

            _logger.LogInformation($"Stopping {ExecuteFile}");
            try { process.Kill(); } catch { }
            _logger.LogInformation($"Processed killed, {ExecuteFile}");
        }

        private Process BuildProcess()
        {
            var process = new Process()
            {
                EnableRaisingEvents = true,

                StartInfo = new ProcessStartInfo
                {
                    FileName = ExecuteFile,
                    Arguments = Arguments,
                    WorkingDirectory = WorkingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };

            process.OutputDataReceived += (s, e) => LogOutput(e.Data, false);
            process.ErrorDataReceived += (s, e) => LogOutput(e.Data, true);

            return process;
        }

        private void LogOutput(string data, bool isError)
        {
            if (data == null) return;

            string message = $"LocalProcess: ({(isError ? "err" : "std")}) {data}";
            _logger.LogInformation(message);

            CaptureOutput?.Invoke(data);
        }
    }
}
