using Microsoft.Extensions.Configuration;
using MlHostSdk.Package;
using MlHostSdk.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Toolbox.Tools;

namespace MlHostCli.Application
{
    internal static class OptionExtensions
    {
        private static readonly IReadOnlyList<Action<Option>> _scenarios = new List<Action<Option>>
        {
            // Scenarios are listed in priority order
            option => Verify(option.Upload, () =>
            {
                LoadSpecFile(option);
                option.PackageFile.VerifyNotEmpty($"{nameof(option.PackageFile)} file is required for {nameof(option.Upload)}");
                option.PackageFile.VerifyAssert(x => File.Exists(x), $"{nameof(option.PackageFile)} file does not exist");
                VerifyModelId(option);
                VerifyStore(option);
            }),

            option => Verify(option.Download, () =>
            {
                option.PackageFile.VerifyNotEmpty($"{nameof(option.PackageFile)} is required for {nameof(option.Download)}");
                option.PackageFile.VerifyAssert(x => option.Force || !File.Exists(x), $"{nameof(option.PackageFile)} file exist.  To overwrite use the 'Force' option");
                VerifyModelId(option);
                VerifyStore(option);
            }),

            option => Verify(option.Delete, () =>
            {
                VerifyModelId(option);
                VerifyStore(option);
            }),

            option => Verify(option.Bind, () =>
            {
                option.VsProject.VerifyNotEmpty($"{nameof(option.VsProject)} is required");
                option.VsProject.VerifyAssert(x => Path.GetExtension(x).ToNullIfEmpty() != null, $"{option.VsProject} is not a VS CS project file");
                VerifyModelId(option);
                VerifyStore(option);
            }),

            option => Verify(option.Swagger, () =>
            {
                option.ModelName?.ToLower().VerifyStoreVector($"{nameof(option.ModelName)}  is required");
                option.Environment?.ToLower().VerifyStoreVector($"{nameof(option.Environment)}  is required");
                option.SwaggerFile.VerifyNotEmpty($"{nameof(option.SwaggerFile)} is required");
            }),

            option => Verify(option.Build, () =>
            {
                option.SpecFile.VerifyNotEmpty($"{nameof(option.SpecFile)} is required");
                option.SpecFile.VerifyAssert(x => File.Exists(x), $"{option.SpecFile} does not exist");
            }),
        };

        public static Option Verify(this Option option)
        {
            _scenarios
                .ForEach(x => x(option));

            (
                option.Upload ||
                option.Download ||
                option.Delete ||
                option.Swagger ||
                option.Build
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

        private static void Verify(bool test, Action testOption)
        {
            if (test) testOption();
        }

        private static void LoadSpecFile(Option option)
        {
            if (option.SpecFile.IsEmpty()) return;

            MlPackageSpec mlPackageSpec = MlPackageBuilder.ReadBuildFile(option.SpecFile!);

            option.PackageFile = option.PackageFile.ToNullIfEmpty() ?? mlPackageSpec.PackageFile;
            option.ModelName = option.ModelName.ToNullIfEmpty() ?? mlPackageSpec.Manifest?.ModelName;
            option.VersionId = option.VersionId.ToNullIfEmpty() ?? mlPackageSpec.Manifest?.VersionId;
        }

        private static bool VerifyModelId(IOption option)
        {
            option.ModelName?.ToLower().VerifyStoreVector($"{nameof(option.ModelName)}  is required");
            option.VersionId?.ToLower().VerifyStoreVector($"{nameof(option.VersionId)}  is required");
            return true;
        }

        private static bool VerifyStore(IOption option)
        {
            option.Store.VerifyNotNull($"{nameof(option.Store)} has not been specified");

            option.Store?.ContainerName.VerifyNotEmpty($"{nameof(option.Store)}:{nameof(option.Store.ContainerName)} is required");
            option.Store?.AccountName.VerifyNotEmpty($"{nameof(option.Store)}:{nameof(option.Store.AccountName)} is required");
            option.Store?.AccountKey.VerifyNotEmpty($"{nameof(option.Store)}:{nameof(option.Store.AccountKey)} is required");

            return true;
        }
    }
}
