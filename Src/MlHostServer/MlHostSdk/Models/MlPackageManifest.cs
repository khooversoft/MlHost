using Azure.Storage.Blobs.Models;
using MlHostSdk.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostSdk.Models
{
    public class MlPackageManifest
    {
        public string PackageVersion { get; set; } = "1.0.0.0";

        public string ModelName { get; set; } = null!;

        public string VersionId { get; set; } = null!;

        public string RunCmd { get; set; } = null!;

        public string StartSignal { get; set; } = null!;
    }
}
