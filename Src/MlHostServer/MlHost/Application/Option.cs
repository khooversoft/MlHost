using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using Toolbox.Application;
using Toolbox.Tools;

namespace MlHost.Application
{
    internal class Option : IOption
    {
        public string ServiceUri { get; set; } = "http://localhost:5003/predict";

        public string? PackageFile { get; set; }

        public bool KillProcess { get; set; } = true;

        public int MaxRequests { get; set; } = 3;

        public string? LogFile { get; set; }

        public string Environment { get; set; } = "dev";

        public RunEnvironment RunEnvironment { get; set; } = RunEnvironment.Unknown;

        public int Port { get; set; } = 5000;
    }
}
