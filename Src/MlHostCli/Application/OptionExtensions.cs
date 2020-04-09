using Microsoft.Extensions.Configuration;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Toolbox.Tools;

namespace MlHostCli.Application
{
    internal static class OptionExtensions
    {
        private static readonly Func<Option, bool> _verifyStore = x =>
            x.Store.VerifyNotNull($"{nameof(x.Store)} has not been specified") != null
            && x.Store?.ContainerName.VerifyNotEmpty($"{nameof(x.Store)}:{nameof(x.Store.ContainerName)} is required") != null
            && x.Store?.AccountName.VerifyNotEmpty($"{nameof(x.Store)}:{nameof(x.Store.AccountName)} is required") != null
            && x.Store?.AccountKey.VerifyNotEmpty($"{nameof(x.Store)}:{nameof(x.Store.AccountKey)} is required") != null;

        private static readonly Func<Option, bool> _verifyModelId = x =>
            x.ModelName?.ToLower().VerifyStoreVector($"{nameof(x.ModelName)}  is required") != null
            && x.VersionId?.ToLower().VerifyStoreVector($"{nameof(x.VersionId)}  is required") != null;

        private static readonly IReadOnlyList<Func<Option, bool>> _scenarios = new List<Func<Option, bool>>
        {
            // Scenarios are listed in priority order
            option => option.Upload
                && option.PackageFile.VerifyNotEmpty($"{nameof(option.PackageFile)} file is required for {nameof(option.Upload)}") != null
                && option.PackageFile.VerifyAssert(x => File.Exists(x), $"{nameof(option.PackageFile)} file does not exist") != null
                && _verifyModelId(option)
                && _verifyStore(option),

            option => option.Download
                && option.PackageFile.VerifyNotEmpty($"{nameof(option.PackageFile)} is required for {nameof(option.Download)}") != null
                && option.PackageFile.VerifyAssert(x => option.Force || !File.Exists(x), $"{nameof(option.PackageFile)} file exist.  To overwrite use the 'Force' option") != null
                && _verifyModelId(option)
                && _verifyStore(option),

            option => option.Delete
                && _verifyModelId(option)
                && _verifyStore(option),

            option => option.Bind
                && option.VsProject.VerifyNotEmpty($"{nameof(option.VsProject)} is required") != null
                && option.VsProject.VerifyAssert(x => Path.GetExtension(x).ToNullIfEmpty() != null, $"{option.VsProject} is not a VS CS project file") != null
                && _verifyModelId(option)
                && _verifyStore(option),
        };

        public static Option Verify(this Option option)
        {
            _scenarios
                .Select(x => x(option) ? 1 : 0)
                .Sum()
                .VerifyAssert(x => x != 0, $"Unknown command(s).  Use 'help' to list valid commands");

            return option;
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
