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

        public bool Activate { get; set; }

        public bool List { get; set; }

        public bool Force { get; set; }

        public string ZipFile { get; set; } = null!;

        public string ModelName { get; set; } = null!;

        public string VersionId { get; set; } = null!;

        public string HostName { get; set; } = null!;

        public string SecretId { get; set; } = null!;

        public BlobStoreOption BlobStore { get; set; } = null!;
    }
}
