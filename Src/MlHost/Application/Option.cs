using System;
using System.IO;
using System.Reflection;
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
    }
}
