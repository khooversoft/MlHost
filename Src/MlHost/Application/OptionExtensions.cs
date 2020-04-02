using Microsoft.Extensions.Configuration;
using MlHostApi.Option;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHost.Application
{
    internal static class OptionExtensions
    {
        public static void Verify(this Option option)
        {
            option.VerifyNotNull(nameof(option));

            option.ServiceUri.VerifyNotEmpty($"{nameof(option.ServiceUri)} is missing");
            option.HostName.VerifyNotEmpty($"{nameof(option.HostName)} is missing");

            //option.BlobStore!.Verify();
            //option.Deployment!.Verify();

            //if (option.BlobStore!.AccountKey.ToNullIfEmpty() == null)
            //{
            //    option.KeyVault!.Verify();
            //}
        }

        public static void Verify(this DeploymentOption deploymentOption)
        {
            deploymentOption.VerifyNotNull("Deployment option is required");
            deploymentOption.DeploymentFolder.VerifyNotEmpty($"{nameof(deploymentOption.DeploymentFolder)} is missing");
            deploymentOption.PackageFolder.VerifyNotEmpty($"{nameof(deploymentOption.PackageFolder)} is missing");
        }

        public static Option Bind(this IConfiguration configuration)
        {
            configuration.VerifyNotNull(nameof(configuration));

            var option = new Option();
            configuration.Bind(option, x => x.BindNonPublicProperties = true);
            return option;
        }
    }
}
