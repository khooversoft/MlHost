using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostCli.Application
{
    internal class Option : IOption
    {
        public bool Help { get; set; }

        public bool Upload { get; set; }
        public bool Download { get; set; }
        public bool Delete { get; set; }
        public bool List { get; set; }

        public bool Activate { get; set; }
        public bool Deactivate { get; set; }

        public bool Force { get; set; }

        public string? ModelName { get; set; }
        public string? VersionId { get; set; }

        public string? HostName { get; set; }

        public string? SecretId { get; set; }

        public string? ZipFile { get; set; }

        public BlobStoreOption? BlobStore { get; set; }

        public KeyVaultOption? KeyVault { get; set; }
    }
}
