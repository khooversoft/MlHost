using Microsoft.Extensions.Configuration;
using MlHostSdk.Models;
using MlHostSdk.Package;
using MlHostSdk.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Toolbox.Tools;
using Toolbox.Models;
using Toolbox.Application;
using System.Security.Cryptography.X509Certificates;

namespace MlHostCli.Application
{
    internal static class OptionExtensions
    {
        private const string baseId = "MlHostCli.Configs";

        private static readonly IReadOnlyList<Action<Option>> _scenarios = new List<Action<Option>>
        {
            // Scenarios are listed in priority order
            option => RuleFor(option.Upload, () =>
            {
                option.PackageFile.VerifyNotEmpty($"{nameof(option.PackageFile)} file is required for {nameof(option.Upload)}");
                if( !option.Build ) option.PackageFile.VerifyAssert(x => File.Exists(x), $"{nameof(option.PackageFile)} file does not exist");
                VerifyModelId(option);
                option?.Store.Verify();
            }),

            option => RuleFor(option.Download, () =>
            {
                option.PackageFile.VerifyNotEmpty($"{nameof(option.PackageFile)} is required for {nameof(option.Download)}");
                option.PackageFile.VerifyAssert(x => option.Force || !File.Exists(x), $"{nameof(option.PackageFile)} file exist.  To overwrite use the 'Force' option");
                VerifyModelId(option);
                option?.Store.Verify();
            }),

            option => RuleFor(option.Delete, () =>
            {
                VerifyModelId(option);
                option?.Store.Verify();
            }),

            option => RuleFor(option.Bind, () =>
            {
                option.VsProject.VerifyNotEmpty($"{nameof(option.VsProject)} is required");
                option.VsProject.VerifyAssert(x => Path.GetExtension(x).ToNullIfEmpty() != null, $"{option.VsProject} is not a VS CS project file");
                VerifyModelId(option);
                option?.Store.Verify();
            }),

            option => RuleFor(option.Swagger, () =>
            {
                option.ModelName?.ToLower().VerifyStoreVector($"{nameof(option.ModelName)}  is required");
                option.Environment?.ToLower().VerifyStoreVector($"{nameof(option.Environment)}  is required");
                option.SwaggerFile.VerifyNotEmpty($"{nameof(option.SwaggerFile)} is required");
            }),

            option => RuleFor(option.Build, () =>
            {
                option.SpecFile.VerifyNotEmpty($"{nameof(option.SpecFile)} is required");
                option.SpecFile.VerifyAssert(x => File.Exists(x), $"{option.SpecFile} does not exist");
            }),
        };

        public static Option Verify(this Option option)
        {
            LoadSpecFile(option);

            _scenarios
                .ForEach(x => x(option));

            (
                option.Bind ||
                option.Build ||
                option.Delete ||
                option.Download ||
                option.Swagger ||
                option.Upload
            )
            .VerifyAssert(x => x, $"Unknown command(s).  Use 'help' to list valid commands");

            return option;
        }

        public static Option Bind(this IConfiguration configuration)
        {
            configuration.VerifyNotNull(nameof(configuration));

            var option = new Option();
            configuration.Bind(option, x => x.BindNonPublicProperties = true);
            return option;
        }

        public static string ConvertToResourceId(this RunEnvironment subject) => subject switch
        {
            RunEnvironment.Dev => $"{baseId}.dev-config.json",
            RunEnvironment.Acpt => $"{baseId}.acpt-config.json",
            RunEnvironment.Prod => $"{baseId}.prod-config.json",

            _ => throw new InvalidOperationException(),
        };

        private static void RuleFor(bool test, Action testOption)
        {
            if (test) testOption();
        }

        private static void LoadSpecFile(Option option)
        {
            if (option.SpecFile.IsEmpty()) return;

            MlPackageSpec mlPackageSpec = MlPackageBuilder.ReadPackageSpec(option.SpecFile!);

            option.PackageFile = option.PackageFile.ToNullIfEmpty() ?? mlPackageSpec.PackageFile switch { string v => getPackageFile(option.SpecFile!, v), _ => null };
            option.ModelName = option.ModelName.ToNullIfEmpty() ?? mlPackageSpec.Manifest?.ModelName;
            option.VersionId = option.VersionId.ToNullIfEmpty() ?? mlPackageSpec.Manifest?.VersionId;

            static string getPackageFile(string specFile, string packageFile)
            {
                string basePath = Path.GetDirectoryName(Path.GetFullPath(specFile))!;
                return Path.Combine(basePath, packageFile);
            }
        }

        private static bool VerifyModelId(IOption option)
        {
            option.ModelName?.ToLower().VerifyStoreVector($"{nameof(option.ModelName)}  is required");
            option.VersionId?.ToLower().VerifyStoreVector($"{nameof(option.VersionId)}  is required");
            return true;
        }
    }
}
