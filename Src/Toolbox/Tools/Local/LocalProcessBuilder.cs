using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;

namespace Toolbox.Tools.Local
{
    public class LocalProcessBuilder
    {
        public LocalProcessBuilder() { }

        public LocalProcessBuilder(LocalProcessBuilder clone)
        {
            clone.VerifyNotNull(nameof(clone));

            SuccessExitCode = clone.SuccessExitCode;
            ExecuteFile = clone.ExecuteFile;
            Arguments = clone.Arguments;
            WorkingDirectory = clone.WorkingDirectory;
            CaptureOutput = clone.CaptureOutput;
            OnExit = clone.OnExit;
        }

        public int? SuccessExitCode { get; set; }

        public string? ExecuteFile { get; set; }

        public string? Arguments { get; set; }

        public string? WorkingDirectory { get; set; }

        public Action<string>? CaptureOutput { get; set; }

        public Action? OnExit { get; set; }

        public LocalProcessBuilder SetSuccessExitCode(int code)
        {
            SuccessExitCode = code;
            return this;
        }

        public LocalProcessBuilder SetExecuteFile(string executeFile)
        {
            executeFile.VerifyNotEmpty(nameof(executeFile));

            ExecuteFile = executeFile;
            return this;
        }

        public LocalProcessBuilder SetArguments(string arguments)
        {
            arguments.VerifyNotEmpty(nameof(arguments));

            Arguments = arguments;
            return this;
        }

        public LocalProcessBuilder SetWorkingDirectory(string workingDirectory)
        {
            workingDirectory.VerifyNotEmpty(nameof(workingDirectory));

            WorkingDirectory = workingDirectory;
            return this;
        }

        public LocalProcessBuilder SetCaptureOutput(Action<string> captureOutput)
        {
            captureOutput.VerifyNotNull(nameof(captureOutput));
            CaptureOutput = captureOutput;
            return this;
        }

        public LocalProcessBuilder SetOnExit(Action onExit)
        {
            OnExit = onExit;
            return this;
        }

        public override string ToString() => this.GetConfigValues()
                .Aggregate(string.Empty, (a, x) => ", " + a);

        public LocalProcess Build(ILogger logger) => new LocalProcess(this, logger);
    }
}