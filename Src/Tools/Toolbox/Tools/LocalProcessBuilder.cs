using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Tools
{
    public class LocalProcessBuilder
    {
        public string? ExecuteFile { get; set; }

        public string? Arguments { get; set; }

        public string? WorkingDirectory { get; set; }

        public Action<string>? CaptureOutput { get; set; }

        public LocalProcessBuilder SetExecuteFile(string executeFile) { ExecuteFile = executeFile; return this; }

        public LocalProcessBuilder SetArguments(string arguments) { Arguments = arguments; return this; }

        public LocalProcessBuilder SetWorkingDirectory(string workingDirectory) { WorkingDirectory = workingDirectory; return this; }

        public LocalProcessBuilder SetCaptureOutput(Action<string> captureOutput) { CaptureOutput = captureOutput; return this; }

        public LocalProcess Build(ILogger<LocalProcess> logger) => new LocalProcess(this, logger);
    }
}
