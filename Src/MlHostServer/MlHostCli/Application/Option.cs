using MlHostSdk.Package;
using System.Collections;
using System.Collections.Generic;
using Toolbox.Models;
using Toolbox.Services;

namespace MlHostCli.Application
{
    internal class Option : IOption
    {
        public bool Help { get; set; }

        public string? ConfigFile { get; set; }

        public bool Upload { get; set; }
        public bool Download { get; set; }
        public bool Delete { get; set; }
        public bool Swagger { get; set; }
        public bool Build { get; set; }

        public bool Bind { get; set; }
        public string? VsProject { get; set; }

        public bool Force { get; set; }

        public string? ModelName { get; set; }
        public string? VersionId { get; set; }

        public string? SecretId { get; set; }

        public string? PackageFile { get; set; }

        public string? Environment { get; set; }
        public string? SwaggerFile { get; set; }
        public string? SpecFile { get; set; }

        public StoreOption? Store { get; set; }

        public KeyVaultOption? KeyVault { get; set; }

        public ISecretFilter? SecretFilter { get; set; }

        public IPropertyResolver? PropertyResolver { get; set; }
    }

}
