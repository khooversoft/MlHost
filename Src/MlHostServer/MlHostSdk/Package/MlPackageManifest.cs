using MlHostSdk.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostSdk.Package
{
    public partial class MlPackageManifest
    {
        public string PackageVersion { get; set; } = "1.0.0.0";

        public string ModelName { get; set; } = null!;

        public string VersionId { get; set; } = null!;

        public string RunCmd { get; set; } = null!;
    }
}
