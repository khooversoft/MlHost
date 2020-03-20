using MlHostCli.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostCli.Test.Application
{
    internal class TestOption : IOption
    {
        public bool Activate { get; set; }

        public BlobStoreOption BlobStore { get; set; } = null!;

        public bool Delete { get; set; }

        public bool Download { get; set; }

        public bool Force { get; set; }

        public bool Help { get; set; }

        public string HostName { get; set; } = null!;

        public bool List { get; set; }

        public string ModelName { get; set; } = null!;

        public string SecretId { get; set; } = null!;

        public bool Upload { get; set; }

        public string VersionId { get; set; } = null!;

        public string ZipFile { get; set; } = null!;
    }
}
