//using Microsoft.Extensions.Logging;
//using System;
//using System.Diagnostics;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Toolbox.Tools
//{
//    public class LocalProcess_old : IDisposable
//    {
//        private readonly ILogger<LocalProcess> _logger;
//        private TaskCompletionSource<LocalProcess>? _tcs;
//        private CancellationTokenSource? _tokenSource;

//        public LocalProcess_old(ILogger<LocalProcess> logger)
//        {
//            _logger = logger;
//        }

//        public int? SuccessExitCode { get; set; }

//        public string? File { get; set; }

//        public string? Arguments { get; set; }

//        public string? WorkingDirectory { get; set; }

//        public TimeSpan? MaxRunTime { get; set; }

//        public Action<string>? CaptureOutput { get; set; }

//        public int? ExitCode { get; private set; }

//        public Process? Process { get; private set; }

//        public bool IsRunning => Process?.HasExited == false && _tcs != null;

//        public Task<LocalProcess> Run(CancellationToken cancellationToken)
//        {
//            File.VerifyNotEmpty(nameof(File));
//            if (_tcs != null || _tokenSource != null) throw new InvalidOperationException("Local process is running");

//            Process = new Process()
//            {
//                EnableRaisingEvents = true,

//                StartInfo = new ProcessStartInfo
//                {
//                    FileName = File,
//                    Arguments = Arguments,
//                    WorkingDirectory = WorkingDirectory,
//                    UseShellExecute = false,
//                    CreateNoWindow = true,
//                    RedirectStandardOutput = true,
//                    RedirectStandardError = true,
//                }
//            };

//            _logger.LogTrace($"{nameof(LocalProcess)}: Starting local process, File={File}, Arguments={Arguments}, WorkingDirectory={WorkingDirectory}");

//            Process.OutputDataReceived += (s, e) => LogOutput(e.Data);
//            Process.ErrorDataReceived += (s, e) => LogOutput(e.Data);
//            Process.Exited += OnProcessExit;

//            _tcs = new TaskCompletionSource<LocalProcess>();

//            _tokenSource = MaxRunTime != null 
//                ? CancellationTokenSource.CreateLinkedTokenSource(RegisterTimeout(_tcs).Token)
//                : CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

//            _tokenSource.Token.Register(() =>
//            {
//                _logger.LogTrace($"{nameof(LocalProcess)}: Canceled local process, File={File}");
//                Stop();
//            });

//            // Start process
//            if (!Process.Start())
//            {
//                string msg = $"{nameof(LocalProcess)}: Canceled local process, File={File}";
//                _logger.LogError(msg);
//                throw new InvalidOperationException(msg);
//            }

//            Process.BeginOutputReadLine();
//            Process.BeginErrorReadLine();

//            return _tcs.Task;
//        }

//        public void Stop()
//        {
//            Interlocked.Exchange(ref _tokenSource, null!)?.Dispose();

//            try { Process?.Kill(true); } catch { }
//            try { Process?.Close(); } catch { }

//            Interlocked.Exchange(ref _tcs, null!)?.SetResult(this);
//        }

//        public void Dispose() => Stop();

//        public void ClearMaxRunTime() => MaxRunTime = null;

//        private void OnProcessExit(object? sender, EventArgs args)
//        {
//            if (Process == null) throw new ArgumentNullException(nameof(Process));
//            if (_tcs == null) throw new ArgumentNullException(nameof(_tcs));

//            _logger.LogInformation("Process has exit");

//            ExitCode = Process.ExitCode;

//            switch (Process.ExitCode)
//            {
//                case int v when v == -1:
//                    Process.Close();
//                    Process.Dispose();
//                    break;

//                case int v when v != (SuccessExitCode ?? 0):
//                    var errorMessage = Process.StandardError.ReadToEnd();
//                    string msg = $"{nameof(LocalProcess)}: Exit code: {ExitCode} does not match required exit code {SuccessExitCode}, ErrorMessage={errorMessage}";
//                    _logger.LogError(msg);
//                    _tcs.SetException(new ArgumentException(msg));
//                    break;

//                default:
//                    Process.Close();
//                    Process.Dispose();
//                    _tcs.SetResult(this);
//                    break;
//            }

//            Interlocked.Exchange(ref _tokenSource, null!)?.Dispose();
//        }

//        private void LogOutput(string data)
//        {
//            if (data == null) return;

//            string message = $"LocalProcess: {data}";
//            _logger.LogInformation(message);

//            CaptureOutput?.Invoke(message);
//        }

//        private CancellationTokenSource RegisterTimeout(TaskCompletionSource<LocalProcess> tcs)
//        {
//            var tokenSource = new CancellationTokenSource((TimeSpan)MaxRunTime!);

//            tokenSource.Token.Register(() =>
//            {
//                string msg = $"{File} failed to complete within max run time of {(TimeSpan)MaxRunTime}";
//                LoggerExtensions.LogError(_logger, msg);
//                tcs.SetException(new TimeoutException(msg));
//            });

//            return tokenSource;
//        }
//    }
//}
