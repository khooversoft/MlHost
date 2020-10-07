using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using Toolbox.Application;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHost.Application
{
    internal class Option : IOption
    {
        public string Environment { get; set; } = "dev";
        public bool KillProcess { get; set; } = false;
        public string? LogFile { get; set; }
        public int MaxRequests { get; set; } = 3;
        public int ModelPort { get; set; } = 5003;
        public string? PackageFile { get; set; }
        public int Port { get; set; } = 5000;
        public IPropertyResolver PropertyResolver { get; set; } = null!;
        public RunEnvironment RunEnvironment { get; set; } = RunEnvironment.Unknown;
        public string ServiceUri { get; set; } = "http://localhost:{modelPort}/predict";
    }
}
