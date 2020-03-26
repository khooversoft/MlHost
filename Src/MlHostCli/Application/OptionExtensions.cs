using Microsoft.Extensions.Configuration;
using MlHostApi.Option;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MlHostCli.Application
{
    internal static class OptionExtensions
    {
        private static readonly Func<Option, bool> _verifyBlob = x =>
            x.BlobStore.VerifyNotNull($"{nameof(x.BlobStore)} has not been specified") != null
            && x.BlobStore?.ContainerName.VerifyNotEmpty($"{nameof(x.BlobStore)}:{nameof(x.BlobStore.ContainerName)} is required") != null
            && x.BlobStore?.AccountName.VerifyNotEmpty($"{nameof(x.BlobStore)}:{nameof(x.BlobStore.AccountName)} is required") != null
            && x.BlobStore?.AccountKey.VerifyNotEmpty($"{nameof(x.BlobStore)}:{nameof(x.BlobStore.AccountKey)} is required") != null;

        private static readonly Func<Option, bool> _verifyModelId = x =>
            x.ModelName?.ToLower().VerifyBlobVector($"{nameof(x.ModelName)}  is required") != null
            && x.VersionId?.ToLower().VerifyBlobVector($"{nameof(x.VersionId)}  is required") != null;

        private static readonly IReadOnlyList<Func<Option, bool>> _scenarios = new List<Func<Option, bool>>
        {
            // Scenarios are listed in priority order
            option => option.List
                && _verifyBlob(option),

            option => option.Upload
                && option.PackageFile.VerifyNotEmpty($"{nameof(option.PackageFile)} file is required for {nameof(option.Upload)}") != null
                && option.PackageFile.VerifyAssert(x => File.Exists(x), $"{nameof(option.PackageFile)} file does not exist") != null
                && option.PackageFile.VerifyAssert(x => Path.GetExtension(x)?.Equals(".mlPackage", StringComparison.OrdinalIgnoreCase) == true, $"{nameof(option.PackageFile)} is not a 'mlPackage' file") != null
                && _verifyModelId(option)
                && _verifyBlob(option),

            option => option.Download
                && option.PackageFile.VerifyNotEmpty($"{nameof(option.PackageFile)} is required for {nameof(option.Download)}") != null
                && option.PackageFile.VerifyAssert(x => option.Force || !File.Exists(x), $"{nameof(option.PackageFile)} file exist.  To overwrite use the 'Force' option") != null
                && option.PackageFile.VerifyAssert(x => Path.GetExtension(x)?.Equals(".mlPackage", StringComparison.OrdinalIgnoreCase) == true, $"{nameof(option.PackageFile)} is not a 'mlPackage' file") != null
                && _verifyModelId(option)
                && _verifyBlob(option),

            option => option.Delete
                && _verifyModelId(option)
                && _verifyBlob(option),

            option => option.Activate
                && option.HostName.VerifyNotEmpty($"{nameof(option.HostName)} is required") != null
                && _verifyModelId(option)
                && _verifyBlob(option),

            option => option.Deactivate
                && option.HostName.VerifyNotEmpty($"{nameof(option.HostName)} is required") != null
                && _verifyModelId(option)
                && _verifyBlob(option),
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
