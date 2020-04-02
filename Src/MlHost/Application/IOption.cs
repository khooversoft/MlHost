using Toolbox.Repository;

namespace MlHost.Application
{
    public interface IOption
    {
        string? ServiceUri { get; }

        bool ForceDeployment { get; }

        StoreOption? Store { get; }

        DeploymentOption Deployment { get; }

        string? HostName { get; }
    }
}