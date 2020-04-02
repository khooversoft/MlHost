﻿using Toolbox.Repository;

namespace MlHost.Application
{
    public interface IOption
    {
        string? ServiceUri { get; }

        bool ForceDeployment { get; }

        BlobStoreOption? BlobStore { get; }

        DeploymentOption Deployment { get; }

        string? HostName { get; }
    }
}