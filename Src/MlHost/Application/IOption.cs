using Toolbox.Application;

namespace MlHost.Application
{
    public interface IOption
    {
        string Environment { get; }
        bool KillProcess { get; }
        string? LogFile { get; }
        int MaxRequests { get; }
        string? PackageFile { get; }
        int Port { get; }
        RunEnvironment RunEnvironment { get; }
        string ServiceUri { get; }
    }
}