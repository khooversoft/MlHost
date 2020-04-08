using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace Toolbox.Tools
{
    public static class ZipArchiveTools
    {
        public static string ExtractZipToTempDirectory(this string zipFilePath, CancellationToken token)
        {
            string toFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), Path.GetFileNameWithoutExtension(zipFilePath));
            ExtractFromZipFile(zipFilePath, toFolder, token);

            return toFolder;
        }

        public static void ExtractFromZipFile(string zipFilePath, string toFolder, CancellationToken token)
        {
            zipFilePath.VerifyNotEmpty(nameof(zipFilePath))
                .VerifyAssert(x => File.Exists(x), $"{zipFilePath} does not exist");

            using var stream = new FileStream(zipFilePath, FileMode.Open);
            using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, false);

            zipArchive.ExtractToFolder(toFolder, token);
        }

        public static void ExtractZipFileFromResource(Type type, string resourceId, string toFolder, CancellationToken token)
        {
            using Stream packageStream = FileTools.GetResourceStream(type, resourceId);
            using var zipArchive = new ZipArchive(packageStream, ZipArchiveMode.Read, false);
            zipArchive.ExtractToFolder(toFolder, token);
        }

        public static void ExtractToFolder(this ZipArchive zipArchive, string toFolder, CancellationToken token)
        {
            zipArchive.VerifyNotNull(nameof(zipArchive));
            toFolder.VerifyNotEmpty(nameof(toFolder));

            if (Directory.Exists(toFolder)) FileTools.DeleteDirectory(toFolder);
            Directory.CreateDirectory(toFolder);

            FileEntry[] zipFiles = zipArchive.Entries
                .Where(x => !x.FullName.EndsWith("/"))
                .Select(x => new FileEntry(x))
                .ToArray();

            foreach (FileEntry zipFile in zipFiles)
            {
                if (token.IsCancellationRequested) break;

                Path.Combine(toFolder, zipFile.FilePath)
                    .Action(x => zipFile.ExtractToFile(x));
            }
        }

        private struct FileEntry
        {
            public FileEntry(ZipArchiveEntry zipArchiveEntry)
            {
                FilePath = zipArchiveEntry.FullName.Replace("/", @"\");
                ZipArchiveEntry = zipArchiveEntry;
            }

            public string FilePath { get; }

            public ZipArchiveEntry ZipArchiveEntry { get; }

            public void ExtractToFile(string filePath)
            {
                filePath.VerifyNotEmpty(nameof(filePath));

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                ZipArchiveEntry.ExtractToFile(filePath, true);
            }
        }
    }
}
