using MlHostApi.Option;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Application
{
    internal static class OptionExtensions
    {
        public static void Verify(this Option option)
        {
            option.VerifyNotNull(nameof(option));

            option.ServiceUri.VerifyNotEmpty($"{nameof(option.ServiceUri)} is missing");
            option.BlobStore.VerifyNotNull($"{nameof(option.BlobStore)} is missing");

            option.BlobStore!.Verify();
            option.Deployment!.Verify();
            option.KeyVault!.Verify();
        }

        public static void Verify(this DeploymentOption deploymentOption)
        {
            deploymentOption.VerifyNotNull("Deployment option is required");
            deploymentOption.DeploymentFolder.VerifyNotEmpty($"{nameof(deploymentOption.DeploymentFolder)} is missing");
            deploymentOption.PackageFolder.VerifyNotEmpty($"{nameof(deploymentOption.PackageFolder)} is missing");
        }
    }
}
