using Toolbox.Application;
using Toolbox.Services;

namespace MlHost.Application
{
    public interface IOption
    {
        string Environment { get; }
        bool KillProcess { get; }
        string? LogFile { get; }
        int MaxRequests { get; }
        int ModelPort { get; }
        string? PackageFile { get; }
        int Port { get; }
        IPropertyResolver PropertyResolver { get; }
        RunEnvironment RunEnvironment { get; }
        string ServiceUri { get; }
    }
}