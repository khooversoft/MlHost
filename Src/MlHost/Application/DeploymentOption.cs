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
    internal class DeploymentOption
    {
        public string DeploymentFolder { get; set; } = OptionBuilder.BuildPathRelativeFromExceutingAssembly("MlStorageDeploy");

        public string PackageFolder { get; set; } = OptionBuilder.BuildPathRelativeFromExceutingAssembly("MlStorageZip");
    }
}
