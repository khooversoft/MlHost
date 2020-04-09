using Toolbox.Models;
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

        bool Bind { get; }
        string? VsProject { get; }

        bool Force { get; }

        string? ModelName { get; }
        string? VersionId { get; }

        string? SecretId { get; }

        string? PackageFile { get; }

        StoreOption? Store { get; }

        ISecretFilter? SecretFilter { get; }
    }
}