using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using Toolbox.Models;

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

        public static void ExtractFromZipFile(string zipFilePath, string toFolder, CancellationToken token, Action<FileActionProgress>? monitor = null)
        {
            zipFilePath.VerifyNotEmpty(nameof(zipFilePath))
                .VerifyAssert(x => File.Exists(x), $"{zipFilePath} does not exist");

            using var stream = new FileStream(zipFilePath, FileMode.Open);
            using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, false);

            zipArchive.ExtractToFolder(toFolder, token, monitor);
        }

        public static string ExtractZipFromResource(this Type type, string resourceId, string folder, CancellationToken token)
        {
            string toFolder = Path.Combine(Path.GetTempPath(), $"{folder}_{Guid.NewGuid()}");

            ExtractZipFileFromResource(type, resourceId, toFolder, token);
            return toFolder;
        }

        public static void ExtractZipFileFromResource(this Type type, string resourceId, string toFolder, CancellationToken token, Action<FileActionProgress>? monitor = null)
        {
            using Stream packageStream = FileTools.GetResourceStream(type, resourceId);
            using var zipArchive = new ZipArchive(packageStream, ZipArchiveMode.Read, false);
            zipArchive.ExtractToFolder(toFolder, token, monitor);
        }

        public static void ExtractToFolder(this ZipArchive zipArchive, string toFolder, CancellationToken token, Action<FileActionProgress>? monitor)
        {
            zipArchive.VerifyNotNull(nameof(zipArchive));
            toFolder.VerifyNotEmpty(nameof(toFolder));

            if (Directory.Exists(toFolder)) FileTools.DeleteDirectory(toFolder);
            Directory.CreateDirectory(toFolder);

            FileEntry[] zipFiles = zipArchive.Entries
                .Where(x => !x.FullName.EndsWith("/"))
                .Select(x => new FileEntry(x))
                .ToArray();

            int fileCount = 0;
            foreach (FileEntry zipFile in zipFiles)
            {
                if (token.IsCancellationRequested) break;

                monitor?.Invoke(new FileActionProgress(zipFiles.Length, ++fileCount));

                Path.Combine(toFolder, zipFile.FilePath
                    .VerifyAssert(x => !x.StartsWith("\\"), $"Invalid zip file path {zipFile.FilePath}"))
                    .Action(x => zipFile.ExtractToFile(x));
            }
        }

        public static void CompressFiles(string zipFilePath, CancellationToken token, Action<FileActionProgress>? monitor = null, params CopyTo[] files)
        {
            zipFilePath.VerifyNotEmpty(nameof(zipFilePath));
            files.VerifyAssert(x => x.Length > 0, "No fileFolder(s) specified");

            using var stream = new FileStream(zipFilePath, FileMode.Create);
            using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, false);

            int fileCount = 0;
            foreach (var file in files)
            {
                if (token.IsCancellationRequested) return;

                zipArchive.CreateEntryFromFile(file.Source, file.Destination);

                monitor?.Invoke(new FileActionProgress(files.Length, ++fileCount));
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
