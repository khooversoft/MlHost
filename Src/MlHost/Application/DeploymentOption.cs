using MlHost.Tools;
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

        public void Verify()
        {
            DeploymentFolder.VerifyNotEmpty($"{nameof(DeploymentFolder)} is missing");
            PackageFolder.VerifyNotEmpty($"{nameof(PackageFolder)} is missing");
        }

        public IReadOnlyList<KeyValuePair<string, string>> ToDetails(string path)
        {
            Func<string, string> fmtKey = x => path + ":" + x;

            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(fmtKey(nameof(DeploymentFolder)), DeploymentFolder),
                new KeyValuePair<string, string>(fmtKey(nameof(PackageFolder)), PackageFolder),
            };
        }
    }
}
