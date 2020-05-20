using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Tools;

namespace MlHostCli.Application
{
    internal static class OptionHelpExtensions
    {
        public static IReadOnlyList<string> GetHelp(this IOption _)
        {
            return new[]
            {
                "ML Host command line interface commands",
                "",
                "Help                  : Display help",
                "List                  : List active models",
                "",
                "Upload a ML Model zip package to storage",
                "  Upload              : Upload command",
                "  PackageFile={file}  : ML Model Package (Zip) file to upload",
                "  ModelName={name}    : Model's name",
                "  VersionId={name}    : Model's version",
                "  Force               : (optional) Overwrite blob if already exist",
                "",
                "Download a ML Model zip package to storage",
                "  Download            : Download command",
                "  PackageFile={file}  : ML Model Package (Zip) file to upload",
                "  ModelName={name}    : Model's name",
                "  VersionId={name}    : Model's version",
                "  Force               : (optional) Overwrite zip file if already exist",
                "",
                "Delete a ML Model in storage",
                "  Delete              : Delete command",
                "  ModelName={name}    : Model's name",
                "  VersionId={name}    : Model's version",
                "",
                "Activate a ML Model to be executed by its host",
                "  Activate            : Activate command",
                "  ModelName={name}    : Model's name",
                "  VersionId={name}    : Model's version",
                "  HostName={name}     : Name of the host to run the ML model",
                "",
                "Bind a ML Model to a MlHost.",
                "  Bind                : Bind command",
                "  ModelName={name}    : Model's name",
                "  VersionId={name}    : Model's version",
                "  VsProject={path}    : Path of the VS Project for ASP.NET Core.",
                "",
                "Build Swagger JSON for Azure API Management.",
                "  Swagger             : Swagger command",
                "  ModelName={name}    : Model's name",
                "  Environment={name}  : Environment name (Dev, APCT, Prod).",
                "",
                "",
                "Configuration for BlobStorage",
                "  SecretId={secretId}                       : Use .NET Core configuration secret json file.  SecretId indicates which secret file to use.",
                "",
                "  Store:ContainerName={container name}      : Azure Blob Storage container name (required)",
                "  Store:AccountName={accountName}           : Azure Blob Storage account name (required)",
                "  Store:AccountKey={accountKey}             : Azure Blob Storage account key (required)",
                "",
                "  If 'BlobStore:AccountKey' is not specified then key vault will be used to retrieve the account key.",
                "    KeyVault:KeyVaultName={keyVaultName}    : Name of the Azure key vault (required if 'Store:AccountKey' is not specified",
                "    KeyVault:KeyName={keyName}              : Name of the Azure key vault's key where the 'Store:AcountKey' is stored",
                "",
                "Model ID",
               $"  Model name and version must match {VerifyExtensions.ValidPattern}.",
            };
        }

        public static void DumpConfigurations(this IOption option)
        {
            const int maxWidth = 80;

            option.GetConfigValues()
                .Select(x => "  " + x)
                .Prepend(new string('=', maxWidth))
                .Prepend("Current configurations")
                .Prepend(string.Empty)
                .Append(string.Empty)
                .Append(string.Empty)
                .ForEach(x => Console.WriteLine(option.SecretFilter?.FilterSecrets(x) ?? x));
        }
    }
}
