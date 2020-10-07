using MlHostSdk.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Models;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostSdk.Package
{
    public class MlPackageBuilder
    {
        private string? _specFileBase;

        public static string ManifestFileName { get; } = "mlPackage.manifest.json";

        public MlPackageSpec Option { get; set; } = null!;

        public MlPackageBuilder ReadSpecFile(string specFile)
        {
            specFile.VerifyNotEmpty(nameof(specFile));

            _specFileBase = Path.GetDirectoryName(Path.GetFullPath(specFile));
            Option = ReadPackageSpec(specFile);
            return this;
        }

        public BuildResults Build(Action<FileActionProgress>? monitor = null, CancellationToken token = default)
        {
            _specFileBase.VerifyNotEmpty("Specification file not read");

            Option
                .VerifyNotNull("Option is required")
                .Verify();

            CopyTo[] files = GetFiles()
                .Append(WriteManifest())
                .ToArray();

            string zipFilePath = Path.Combine(_specFileBase, Option.PackageFile);

            ZipArchiveTools.CompressFiles(zipFilePath, token, monitor, files);

            return new BuildResults(Option, files.Length);
        }

        private CopyTo[] GetFiles()
        {
            var baseValues = Option.Copy
                .Select(x => (Source: Path.Combine(_specFileBase, x.Source), x.Destination))
                .Select(x => (x.Source, basePath: Path.GetDirectoryName(x.Source), search: Path.GetFileName(x.Source), destination: x.Destination, wildcard: x.Source.IndexOf('*') >= 0));

            CopyTo[] files = baseValues
                .SelectMany(
                    x => Directory.GetFiles(x.basePath, x.search, x.wildcard ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly),
                    (x, c) => new CopyTo { Source = c, Destination = !x.wildcard ? x.destination : Path.Combine(x.destination, c.Substring(x.basePath.Length + 1)) }
                    )
                .ToArray();

            // Verify any non-wild card files are in the list
            baseValues
                .Where(x => x.wildcard == false && !files.Any(y => x.Source.IndexOf(y.Source) >= 0))
                .Select(x => x.Source)
                .VerifyAssert<IEnumerable<string>, FileNotFoundException>(x => x.Count() == 0, x => $"File required where not found, {string.Join(", ", x)}");

            return files;
        }

        public static MlPackageSpec ReadPackageSpec(string filePath)
        {
            filePath
                .VerifyNotEmpty(nameof(filePath))
                .VerifyAssert(x => File.Exists(x), $"{filePath} does not exist");

            string subject = File.ReadAllText(filePath);
            subject.VerifyNotEmpty($"{filePath} is empty");

            return Json.Default.Deserialize<MlPackageSpec>(subject);
        }

        private CopyTo WriteManifest()
        {
            string filePath = Path.Combine(Path.GetTempPath(), ManifestFileName);
            Option.Manifest.WriteToFile(filePath);

            return new CopyTo
            {
                Source = filePath,
                Destination = ManifestFileName,
            };
        }
    }
}
