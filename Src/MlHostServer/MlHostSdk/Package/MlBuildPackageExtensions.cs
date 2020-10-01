using MlHostSdk.Models;
using System.IO;
using System.Linq;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostSdk.Package
{
    public static class MlBuildPackageExtensions
    {
        public static MlPackageSpec Verify(this MlPackageSpec option)
        {
            option.VerifyNotNull(nameof(option));

            option.Manifest
                .VerifyNotNull("Manifest is required")
                .Verify();

            option.PackageFile.VerifyNotEmpty($"{nameof(option.PackageFile)} is required");

            option.Copy
                .VerifyNotNull($"{nameof(option.Copy)} is required")
                .VerifyAssert(x => x.Count > 0, $"{nameof(option.Copy)} is empty")
                .VerifyAssert(x => x.All(y => !y.Source.IsEmpty()), "One or more source paths are empty")
                .VerifyAssert(x => x.All(y => !y.Destination.IsEmpty()), "One or more destination paths are empty");

            return option;
        }

        public static void Verify(this MlPackageManifest option)
        {
            option.VerifyNotNull(nameof(option));

            option.PackageVersion.VerifyNotEmpty($"{nameof(option.PackageVersion)} is required");
            option.ModelName.VerifyNotEmpty($"{nameof(option.ModelName)} is required");
            option.VersionId.VerifyNotEmpty($"{nameof(option.VersionId)} is required");
            option.RunCmd.VerifyNotEmpty($"{nameof(option.RunCmd)} is required");
            option.StartSignal.VerifyNotEmpty($"{nameof(option.StartSignal)} is required");
        }

        public static void WriteToFile(this MlPackageManifest mlPackageManifest, string filePath)
        {
            filePath.VerifyNotEmpty(nameof(filePath));

            mlPackageManifest.Verify();
            string json = Json.Default.SerializeWithIndent(mlPackageManifest);
            File.WriteAllText(filePath, json);
        }
    }
}
