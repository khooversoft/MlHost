namespace MlHost.Application
{
    public interface IOption
    {
        string? ServiceUri { get; }

        string DeploymentFolder { get; }

        string? PackageFile { get; }

        bool KillProcess { get; }

        int MaxRequests { get; }

        string? LogFile { get; }
    }
}