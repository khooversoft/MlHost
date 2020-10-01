using MlHostSdk.Models;
using System.IO;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostSdk.Types
{
    public class MlPackageManifestFile
    {
        private readonly string _filePath;
        private MlPackageManifest? _manifest;
        private string? _executeCmd;
        private string? _arguments;
        private string? _fileFolder;

        public MlPackageManifestFile(string filePath)
        {
            filePath.VerifyNotEmpty(nameof(filePath))
                .VerifyAssert<string, FileNotFoundException>(x => File.Exists(x), x => $"{x} does not exist");

            _filePath = filePath;
            _fileFolder = Path.GetDirectoryName(_filePath);
        }

        public MlPackageManifest MlPackageManifest => _manifest ?? ReadPackage();

        public string RunCmd => _executeCmd ??= ParseRunCmd(MlPackageManifest);

        public string? Arguments => _arguments ??= ParseRunArguments(MlPackageManifest);

        private MlPackageManifest ReadPackage()
        {
            string json = File.ReadAllText(_filePath);

            return _manifest = Json.Default.Deserialize<MlPackageManifest>(json)
                .VerifyNotNull($"Error in parsing {nameof(MlPackageManifest)}")
                .VerifyAssert(x => !x.RunCmd.IsEmpty(), $"RunCmd is empty in {_filePath}");
        }

        private string ParseRunCmd(MlPackageManifest manifest)
        {
            // Read RunCmd and parse
            int spaceIndex = manifest.RunCmd.IndexOf(' ');
            if (spaceIndex < 0) return Path.Combine(_fileFolder!, manifest.RunCmd!);

            string executePath = manifest.RunCmd.Substring(0, spaceIndex).Trim();
            return Path.Combine(_fileFolder!, executePath);
        }

        private string? ParseRunArguments(MlPackageManifest manifest)
        {
            // Read RunCmd and parse
            int spaceIndex = manifest.RunCmd.IndexOf(' ');
            if (spaceIndex < 0) return null;

            return manifest.RunCmd.Substring(spaceIndex + 1).Trim();
        }
    }
}
