using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MlHostCli.Application
{
    internal static class OptionVerifyExtensions
    {
        private static readonly Func<Option, bool> _verifyBlob = x => 
            x.BlobStore.VerifyNotNull($"{nameof(x.BlobStore)} has not been specified") != null
            && x.BlobStore?.ContainerName.VerifyNotEmpty($"{nameof(x.BlobStore)}:{nameof(x.BlobStore.ContainerName)}") != null
            && x.BlobStore?.ConnectionString.VerifyNotEmpty($"{nameof(x.BlobStore)}:{nameof(x.BlobStore.ConnectionString)}") != null;

        private static readonly Func<Option, bool> _verifyModelId = x =>
            x.ModelName.VerifyBlobVector(nameof(x.ModelName)) != null
            && x.VersionId.VerifyBlobVector(nameof(x.VersionId)) != null;

        private static readonly IReadOnlyList<Func<Option, bool>> _scenarios = new List<Func<Option, bool>>
        {
            // Scenarios are listed in priority order
            option => option.Help,

            option => option.List
                && _verifyBlob(option),

            option => option.Upload
                && option.ZipFile.VerifyNotEmpty($"{nameof(option.ZipFile)} file is required for {nameof(option.Upload)}") != null
                && option.ZipFile.VerifyAssert(x => File.Exists(x), $"{nameof(option.ZipFile)} file does not exist") != null
                && option.ZipFile.VerifyAssert(x => Path.GetExtension(x)?.Equals(".zip", StringComparison.OrdinalIgnoreCase) == true, $"{nameof(option.ZipFile)} is not a zip file") != null
                && _verifyModelId(option)
                && _verifyBlob(option),

            option => option.Download
                && option.ZipFile.VerifyNotEmpty(nameof(option.ZipFile)) != null
                && option.ZipFile.VerifyAssert(x => option.Force || !File.Exists(x), $"{nameof(option.ZipFile)} file exist.  To overwrite use the 'Force' option") != null
                && option.ZipFile.VerifyAssert(x => Path.GetExtension(x)?.Equals(".zip", StringComparison.OrdinalIgnoreCase) == true, $"{nameof(option.ZipFile)} is not a zip file") != null
                && _verifyModelId(option)
                && _verifyBlob(option),

            option => option.Delete
                && _verifyModelId(option)
                && _verifyBlob(option),

            option => option.Activate
                && option.HostName.VerifyNotEmpty(nameof(option.HostName)) != null
                && _verifyModelId(option)
                && _verifyBlob(option),
        };

        public static void Verify(this Option option)
        {
            Func<Option, bool>? result = _scenarios
                .SkipWhile(x => !x(option))
                .FirstOrDefault();

            if (result == null) throw new ArgumentException($"Unknown command(s).  Use help to list valid commands");
        }
    }
}
