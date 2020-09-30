using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private const string _manifestFileName = "mlPackage.manifest.json";
        private string? _specFileBase;

        public MlPackageSpec Option { get; set; } = null!;

        public MlPackageBuilder SetOption(MlPackageSpec option) => this.Action(x => x.Option = option.Verify());

        public MlPackageBuilder ReadOption(string specFile)
        {
            specFile.VerifyNotEmpty(nameof(specFile));

            _specFileBase = Path.GetDirectoryName(specFile);
            Option = ReadBuildFile(specFile);
            return this;
        }

        public int Build(Action<FileActionProgress>? monitor = null, CancellationToken token = default)
        {
            Option
                .VerifyNotNull("Option is required")
                .Verify();

            CopyTo[] files = GetFiles()
                .Append(WriteManifest())
                .ToArray();

            ZipArchiveTools.CompressFiles(Path.Combine(_specFileBase, Option.PackageFile), token, monitor, files);

            return files.Length;
        }

        private CopyTo[] GetFiles()
        {
            return Option.Copy
                .Select(x => (Source: Path.Combine(_specFileBase, x.Source), x.Destination))
                .Select(x => (basePath: Path.GetDirectoryName(x.Source), search: Path.GetFileName(x.Source), destination: x.Destination, wildcard: x.Source.IndexOf('*') >= 0))
                .SelectMany(
                    x => Directory.GetFiles(x.basePath, x.search, x.wildcard ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly),
                    (x, c) => new CopyTo { Source = c, Destination = !x.wildcard ? x.destination : Path.Combine(x.destination, c.Substring(x.basePath.Length + 1)) }
                    )
                .ToArray();
        }

        public static MlPackageSpec ReadBuildFile(string filePath)
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
            string filePath = Path.Combine(Path.GetTempPath(), _manifestFileName);
            Option.Manifest.WriteToFile(filePath);

            return new CopyTo
            {
                Source = filePath,
                Destination = _manifestFileName,
            };
        }
    }
}
