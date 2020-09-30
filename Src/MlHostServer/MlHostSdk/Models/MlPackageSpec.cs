using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Models;

namespace MlHostSdk.Models
{
    public class MlPackageSpec
    {
        public string PackageFile { get; set; } = null!;

        public MlPackageManifest Manifest { get; set; } = null!;

        public IList<CopyTo> Copy { get; set; } = null!;
    }
}
