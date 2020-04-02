using MlHostApi.Option;
using Toolbox.Repository;

namespace MlHost.Application
{
    internal class Option : IOption
    {
        public string ServiceUri { get; set; } = "http://localhost:5003/predict";

        public bool ForceDeployment { get; set; }

        public string? HostName { get; set; }

        public BlobStoreOption? BlobStore { get; set; }

        public DeploymentOption Deployment { get; set; } = new DeploymentOption();

        public KeyVaultOption? KeyVault { get; set; }

        public string? SecretId { get; set; }
    }
}
