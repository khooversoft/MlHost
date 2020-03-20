using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostCli.Application
{
    internal static class OptionHelpExtensions
    {
        public static IReadOnlyList<string> GetHelp(this IOption _)
        {
            return new[]
            {
                "",
                "ML Host command line interface commands",
                "",
               $"                      : Model name and version must match {VerifyExtensions.ValidPattern}.",
                "SecretId={secretId}   : Use .NET Core configuration secret json file.  SecretId indicates which secret file to use.",
                "",
                "Help                  : Display help",
                "List                  : List active models",
                "",
                "Upload a ML Model zip package to storage",
                "  Upload              : Upload command",
                "  ZipFile={file}      : Zip file to upload",
                "  ModelName={name}    : Model's name",
                "  VersionId={name}    : Model's version",
                "  Force               : (optional) Overwrite blob if already exist",
                "",
                "Download a ML Model zip package to storage",
                "  Download              : Download command",
                "  ZipFile={file}      : Zip file to upload",
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
            };
        }
    }
}
