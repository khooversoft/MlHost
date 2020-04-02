using Toolbox.Repository;
using Toolbox.Services;

namespace MlHostCli.Application
{
    internal interface IOption
    {
        bool Help { get; }
        bool Dump { get; }

        bool Upload { get; }
        bool Download { get; }
        bool Delete { get; }
        bool List { get; }
        bool Activate { get; }
        bool Deactivate { get; }

        bool Force { get; }

        string? ModelName { get; }
        string? VersionId { get; }

        string? HostName { get; }

        string? SecretId { get; }

        string? PackageFile { get; }

        BlobStoreOption? BlobStore { get; }

        ISecretFilter? SecretFilter { get; }
    }
}