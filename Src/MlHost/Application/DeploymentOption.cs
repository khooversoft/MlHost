using MlHost.Tools;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MlHost.Application
{
    public class DeploymentOption
    {
        public string DeploymentFolder { get; set; } = "MlStorageDeploy";

        public string PackageFolder { get; set; } = "MlStorageZip";
    }
}
