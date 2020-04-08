using MlHostApi.Tools;
using System.Collections.Generic;

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
                "Dump                  : Dump current configuration",
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
                "  InstallPath={path}  : Path where to install model.",
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
    }
}
