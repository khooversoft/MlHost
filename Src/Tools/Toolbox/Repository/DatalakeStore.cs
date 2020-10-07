using Azure;
using Azure.Storage;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Models;
using Toolbox.Tools;

namespace Toolbox.Repository
{
    public class DatalakeStore : IDatalakeStore
    {
        private readonly DataLakeFileSystemClient _fileSystem;
        private readonly ILogger<DatalakeStore> _logger;
        private readonly DataLakeServiceClient _serviceClient;

        public DatalakeStore(StoreOption blobStoreOption, ILogger<DatalakeStore> logger)
        {
            blobStoreOption.VerifyNotNull(nameof(blobStoreOption)).Verify();
            logger.VerifyNotNull(nameof(blobStoreOption));

            _logger = logger;

            // Create DataLakeServiceClient using StorageSharedKeyCredentials
            var serviceUri = new Uri($"https://{blobStoreOption.AccountName}.blob.core.windows.net");

            StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(blobStoreOption.AccountName, blobStoreOption.AccountKey);
            _serviceClient = new DataLakeServiceClient(serviceUri, sharedKeyCredential);

            // Get a reference to a file system (container)
            _fileSystem = _serviceClient.GetFileSystemClient(blobStoreOption.ContainerName);
        }

        public async Task<bool> Delete(string path, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));

            _logger.LogInformation($"{nameof(Delete)}: Path={path}");

            DataLakeFileClient file = _fileSystem.GetFileClient(path);
            return await file.DeleteIfExistsAsync(cancellationToken: token);
        }

        public async Task DeleteDirectory(string path, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));

            _logger.LogInformation($"{nameof(DeleteDirectory)}: Path={path}");

            DataLakeDirectoryClient directoryClient = _fileSystem.GetDirectoryClient(path);
            await directoryClient.DeleteAsync(cancellationToken: token);
        }

        public async Task Download(string path, Stream toStream, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));
            toStream.VerifyNotNull(nameof(toStream));

            _logger.LogInformation($"{nameof(Download)}: Path={path}");

            DataLakeFileClient file = _fileSystem.GetFileClient(path);
            await file.ReadToAsync(toStream, cancellationToken: token);
        }

        public async Task<bool> Exist(string path, CancellationToken token)
        {
            DataLakeFileClient file = _fileSystem.GetFileClient(path);
            Response<bool> response = await file.ExistsAsync(token);
            return response.Value;
        }

        public async Task<DatalakePathProperties> GetPathProperties(string path, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));

            DataLakeFileClient file = _fileSystem.GetFileClient(path);
            return (await file.GetPropertiesAsync(cancellationToken: token)).Value
                .ConvertTo();
        }

        public async Task<byte[]> Read(string path, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));

            DataLakeFileClient file = _fileSystem.GetFileClient(path);
            Response<FileDownloadInfo> response = await file.ReadAsync(token);

            using MemoryStream memory = new MemoryStream();
            await response.Value.Content.CopyToAsync(memory);

            return memory.ToArray();
        }

        public async Task<IReadOnlyList<DatalakePathItem>> Search(string? path, Func<DatalakePathItem, bool> filter, bool recursive, CancellationToken token)
        {
            var list = new List<DatalakePathItem>();

            await foreach (PathItem pathItem in _fileSystem.GetPathsAsync(path, recursive, cancellationToken: token))
            {
                DatalakePathItem datalakePathItem = pathItem.ConvertTo();

                if (filter(datalakePathItem)) list.Add(datalakePathItem);
            }

            return list;
        }

        public async Task Upload(Stream fromStream, string toPath, bool force, CancellationToken token)
        {
            fromStream.VerifyNotNull(nameof(fromStream));
            toPath.VerifyNotEmpty(nameof(toPath));

            DataLakeFileClient file = _fileSystem.GetFileClient(toPath);
            await file.DeleteIfExistsAsync(cancellationToken: token);

            using var reporter = new Reporter(nameof(Upload), x => $"{(x / 1024):#,###}/KB", _logger);
            await file.UploadAsync(fromStream, progressHandler: reporter, cancellationToken: token);
        }

        public async Task Write(string path, byte[] data, bool force, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));
            data
                .VerifyNotNull(nameof(data))
                .VerifyAssert(x => x.Length > 0, $"{nameof(data)} length must be greater then 0");

            using var memoryBuffer = new MemoryStream(data.ToArray());

            DataLakeFileClient file = _fileSystem.GetFileClient(path);
            await file.UploadAsync(memoryBuffer, force, token);
        }
    }
}