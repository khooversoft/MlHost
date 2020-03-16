using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Tools
{
    internal class PackageUpdate
    {
        private readonly MD5 _md5 = MD5.Create();
        private readonly ILogger _logger;
        private readonly CancellationToken _token;

        public PackageUpdate(ILogger logger, CancellationToken token)
        {
            _logger = logger;
            _token = token;
        }

        public void UpdateToFolder(string deploymentFolder, Type type, string resourceId)
        {
            deploymentFolder = deploymentFolder.ToNullIfEmpty() ?? throw new ArgumentException(nameof(deploymentFolder));
            resourceId = resourceId.ToNullIfEmpty() ?? throw new ArgumentException(nameof(resourceId));

            using Stream? packageStream = Assembly.GetAssembly(type)!.GetManifestResourceStream(resourceId);
            if (packageStream == null) throw new ArgumentException($"Resource ID {resourceId} not located in assembly");

            Directory.CreateDirectory(deploymentFolder);

            using ZipArchive zipArchive = new ZipArchive(packageStream, ZipArchiveMode.Read, false);

            ZipFile[] zipFiles = zipArchive.Entries
                .Where(x => !x.FullName.EndsWith("/"))
                .Select(x => new ZipFile(x))
                .ToArray();

            DeleteFileNotInZip(deploymentFolder, zipArchive, zipFiles);
            UpdateFiles(deploymentFolder, zipArchive, zipFiles);
        }

        private void DeleteFileNotInZip(string deploymentFolder, ZipArchive zipArchive, ZipFile[] zipFiles)
        {
            string[] currentFiles = Directory.GetFiles(deploymentFolder, "*.*", SearchOption.AllDirectories);
            if (currentFiles.Length == 0) return;

            string[] filesToDelete = currentFiles
                .Select(x => x.Substring(deploymentFolder.Length + 1))
                .Except(zipFiles.Select(x => x.FilePath), StringComparer.OrdinalIgnoreCase)
                .Select(x => Path.Combine(deploymentFolder, x))
                .ToArray();

            var tasks = new List<Task>();

            foreach (var file in filesToDelete)
            {
                if (_token.IsCancellationRequested) break;

                tasks.Add(Task.Run(() =>
                {
                    _logger.LogTrace($"Deleting file {file} because its not in package (zip)");
                    File.Delete(file);
                }, _token));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private void UpdateFiles(string deploymentFolder, ZipArchive zipArchive, ZipFile[] zipFiles)
        {
            foreach (ZipFile zipFile in zipFiles)
            {
                if (_token.IsCancellationRequested) break;

                string filePath = Path.Combine(deploymentFolder, zipFile.FilePath);

                if (File.Exists(filePath))
                {
                    if (IsFileCurrent(filePath, zipFile.ZipArchiveEntry))
                    {
                        _logger.LogTrace($"File {filePath} is up to date");
                        continue;
                    }
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }

                _logger.LogTrace($"File {filePath} is created or updated");
                zipFile.ZipArchiveEntry.ExtractToFile(filePath, true);
            }
        }

        private bool IsFileCurrent(string filePath, ZipArchiveEntry zipArchiveEntry)
        {
            using Stream fileStream = new FileStream(filePath, FileMode.Open);
            byte[] fileHash = _md5.ComputeHash(fileStream);

            using Stream zipFileStream = zipArchiveEntry.Open();
            byte[] zipFileHash = _md5.ComputeHash(zipFileStream);

            return Enumerable.SequenceEqual(fileHash, zipFileHash);
        }

        private struct ZipFile
        {
            public ZipFile(ZipArchiveEntry zipArchiveEntry)
            {
                FilePath = zipArchiveEntry.FullName.Replace("/", @"\");
                ZipArchiveEntry = zipArchiveEntry;
            }

            public string FilePath { get; }

            public ZipArchiveEntry ZipArchiveEntry { get; }
        }
    }
}
