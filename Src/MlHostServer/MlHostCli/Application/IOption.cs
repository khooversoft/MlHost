using Toolbox.Application;
using Toolbox.Models;
using Toolbox.Services;

namespace MlHostCli.Application
{
    internal interface IOption
    {
        bool Help { get; }

        bool Upload { get; }
        bool Download { get; }
        bool Delete { get; }
        bool Swagger { get; }
        bool Build { get; }

        bool Bind { get; }
        string? VsProject { get; }

        bool Force { get; }

        string? ModelName { get; }
        string? VersionId { get; }

        string? SecretId { get; }

        string? PackageFile { get; }
        string? SwaggerFile { get; }
        string? SpecFile { get; }

        string? Environment { get; }
        RunEnvironment RunEnvironment { get; }

        StoreOption? Store { get; }

        ISecretFilter? SecretFilter { get; }

        IPropertyResolver? PropertyResolver { get; }
    }
}