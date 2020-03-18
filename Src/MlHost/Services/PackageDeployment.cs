using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Services.PackageSource;
using MlHost.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Services
{
    internal class PackageDeployment : IPackageDeployment
    {
        private readonly MD5 _md5 = MD5.Create();
        private readonly ILogger<PackageDeployment> _logger;
        private readonly IOption _option;
        private readonly IExecutionContext _executionContext;
        private readonly IPackageSource _packageSource;

        public PackageDeployment(ILogger<PackageDeployment> logger, IOption option, IExecutionContext executionContext, IPackageSource packageSource)
        {
            _logger = logger;
            _option = option;
            _executionContext = executionContext;
            _packageSource = packageSource;
        }

        public async Task Deploy()
        {
            bool folderExist = Directory.Exists(_option.Deployment.DeploymentFolder);

            _logger.LogInformation($"Deploying ML code & model, deployment folder={_option.Deployment.DeploymentFolder}, exist={folderExist}, forceDelete={_option.ForceDeployment}");
            if (folderExist && _option.ForceDeployment) ResetDeploymentFolder();

            Directory.CreateDirectory(_option.Deployment.DeploymentFolder);

            await UpdateToFolder();
            _logger.LogInformation($"Deployed ML code & model");
        }

        private void ResetDeploymentFolder()
        {
            _logger.LogInformation($"Deleting exiting deployment folder {_option.Deployment.DeploymentFolder}");
            Directory.Delete(_option.Deployment.DeploymentFolder, true);

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            while (Directory.Exists(_option.Deployment.DeploymentFolder) && !timeout.Token.IsCancellationRequested)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }
        }

        public async Task UpdateToFolder()
        {
            using Stream zipStream = await _packageSource.GetStream();
            using ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read, false);

            ZipFile[] zipFiles = zipArchive.Entries
                .Where(x => !x.FullName.EndsWith("/"))
                .Select(x => new ZipFile(x))
                .ToArray();

            DeleteFileNotInZip(zipFiles);
            UpdateFiles(zipFiles);
        }

        private void DeleteFileNotInZip(ZipFile[] zipFiles)
        {
            string[] currentFiles = Directory.GetFiles(_option.Deployment.DeploymentFolder, "*.*", SearchOption.AllDirectories);
            if (currentFiles.Length == 0) return;

            string[] filesToDelete = currentFiles
                .Select(x => x.Substring(_option.Deployment.DeploymentFolder.Length + 1))
                .Except(zipFiles.Select(x => x.FilePath), StringComparer.OrdinalIgnoreCase)
                .Select(x => Path.Combine(_option.Deployment.DeploymentFolder, x))
                .ToArray();

            Parallel.ForEach(filesToDelete, file =>
            {
                _logger.LogTrace($"Deleting file {file} because its not in package (zip)");
                File.Delete(file);
            });
        }

        private void UpdateFiles(ZipFile[] zipFiles)
        {
            foreach (ZipFile zipFile in zipFiles)
            {
                if (_executionContext.TokenSource.Token.IsCancellationRequested) break;

                string filePath = Path.Combine(_option.Deployment.DeploymentFolder, zipFile.FilePath);

                if (File.Exists(filePath) && IsFileCurrent(filePath, zipFile.ZipArchiveEntry))
                {
                    _logger.LogTrace($"File {filePath} is up to date");
                    continue;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

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
