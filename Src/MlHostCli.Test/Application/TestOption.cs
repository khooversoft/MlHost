using MlHostCli.Application;
using Toolbox.Models;
using Toolbox.Services;

namespace MlHostCli.Test.Application
{
    internal class TestOption : IOption
    {
        public bool Help { get; set; }

        public bool Dump { get; set; }

        public bool Upload { get; set; }

        public bool Download { get; set; }

        public bool Delete { get; set; }

        public bool Bind { get; set; }

        public string? InstallPath { get; set; }

        public bool Force { get; set; }

        public string? ModelName { get; set; }

        public string? VersionId { get; set; }

        public string? SecretId { get; set; }

        public string? PackageFile { get; set; }

        public StoreOption? Store { get; set; }

        public ISecretFilter? SecretFilter { get; set; }
    }
}
