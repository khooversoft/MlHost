using MlHostApi.Option;

namespace MlHost.Application
{
    internal interface IOption
    {
        string? ServiceUri { get; }

        bool ForceDeployment { get; }

        BlobStoreOption? BlobStore { get; }

        DeploymentOption? Deployment { get; }

        string? HostName { get; }
    }
}