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

        public static void ExtractFromZipFile(string zipFilePath, string toFolder, CancellationToken token, Action<ZipExtractProgress>? monitor = null)
        {
            zipFilePath.VerifyNotEmpty(nameof(zipFilePath))
                .VerifyAssert(x => File.Exists(x), $"{zipFilePath} does not exist");

            using var stream = new FileStream(zipFilePath, FileMode.Open);
            using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, false);

            zipArchive.ExtractToFolder(toFolder, token, monitor);
        }

        public static string ExtractZipFromResource(Type type, string resourceId, string folder, string fileName, CancellationToken token)
        {
            string filePath = Path.Combine(Path.GetTempPath(), folder, fileName);

            string directory = Path.GetDirectoryName(filePath)!;
            FileTools.DeleteDirectory(directory);
            Directory.CreateDirectory(directory);

            ExtractZipFileFromResource(type, resourceId, filePath, token);
            return filePath;
        }

        public static void ExtractZipFileFromResource(Type type, string resourceId, string toFolder, CancellationToken token, Action<ZipExtractProgress>? monitor = null)
        {
            using Stream packageStream = FileTools.GetResourceStream(type, resourceId);
            using var zipArchive = new ZipArchive(packageStream, ZipArchiveMode.Read, false);
            zipArchive.ExtractToFolder(toFolder, token, monitor);
        }

        public static void ExtractToFolder(this ZipArchive zipArchive, string toFolder, CancellationToken token, Action<ZipExtractProgress>? monitor)
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

                monitor?.Invoke(new ZipExtractProgress(zipFiles.Length, ++fileCount));

                Path.Combine(toFolder, zipFile.FilePath)
                    .Action(x => zipFile.ExtractToFile(x));
            }
        }

        public static void CompressFiles(string zipFilePath, CancellationToken token, params string[] fileFolders)
        {
            zipFilePath.VerifyNotEmpty(nameof(zipFilePath));
            fileFolders.VerifyAssert(x => x.Length > 0, "No fileFolder(s) specified");

            using var stream = new FileStream(zipFilePath, FileMode.Create);
            using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, false);

            var pushFiles = fileFolders
                .Select(x => getFileFolder(x));

            var stack = new Stack<(string folder, string file)>(pushFiles);

            while(stack.TryPop(out (string folder, string file) fileFolder))
            {
                string filePath = Path.Combine(fileFolder.folder, fileFolder.file);

                FileAttributes attr = File.GetAttributes(filePath);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    pushFolderFiles(filePath);
                    continue;
                }

                ZipArchiveEntry zipArchiveEntry = zipArchive.CreateEntry(fileFolder.file);
                zipArchive.CreateEntryFromFile(Path.Combine(fileFolder.folder, fileFolder.file), fileFolder.file);
            }


            (string folder, string file) getFileFolder(string fileFolder, string? baseFolder = null)
            {
                string folder = Path.GetDirectoryName(baseFolder ?? fileFolder)!;
                string file = fileFolder.Substring(folder.Length);

                return (folder, file);
            }

            void pushFolderFiles(string folderPath)
            {
                string[] files = Directory.GetFiles(folderPath, "*.*");

                files
                    .Select(x => getFileFolder(x, folderPath))
                    .ForEach(stack.Push);
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
