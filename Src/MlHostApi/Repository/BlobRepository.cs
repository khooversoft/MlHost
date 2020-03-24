using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MlHostApi.Repository
{
    public class BlobRepository : IBlobRepository
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string _containerName;
        private readonly string _connectionString;

        public BlobRepository(string containerName, string connectionString)
        {
            containerName.VerifyNotEmpty(nameof(containerName));
            connectionString.VerifyNotEmpty(nameof(connectionString));

            _containerName = containerName;
            _connectionString = connectionString;

            var client = new BlobServiceClient(_connectionString);
            _containerClient = client.GetBlobContainerClient(_containerName);
        }

        public Task Delete(string path, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));

            return _containerClient.DeleteBlobIfExistsAsync(path, DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: token);
        }

        public async Task Download(string path, Stream toStream, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));
            toStream.VerifyNotNull(nameof(toStream));

            BlobClient blobClient = _containerClient.GetBlobClient(path);
            BlobDownloadInfo download = await blobClient.DownloadAsync(token);

            await download.Content.CopyToAsync(toStream);
        }

        public async Task Upload(Stream fromStream, string toPath, bool force, CancellationToken token)
        {
            fromStream.VerifyNotNull(nameof(fromStream));
            toPath.VerifyNotEmpty(nameof(toPath));

            if (force && (await Exist(toPath, token)))
            {
                await Delete(toPath, token);
            }

            await _containerClient.UploadBlobAsync(toPath, fromStream, token);
        }

        public async Task<bool> Exist(string path, CancellationToken token)
        {
            await foreach (BlobItem _ in _containerClient.GetBlobsAsync(prefix: path, cancellationToken: token))
            {
                return true;
            }

            return false;
        }

        public async Task<byte[]> Read(string path, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));

            BlobClient blobClient = _containerClient.GetBlobClient(path);
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using MemoryStream memory = new MemoryStream();
            await download.Content.CopyToAsync(memory);

            return memory.ToArray();
        }

        public async Task Write(string path, byte[] data, CancellationToken token)
        {
            path.VerifyNotEmpty(nameof(path));
            data
                .VerifyNotNull(nameof(data))
                .VerifyAssert(x => x.Length > 0, $"{nameof(data)} length must be greater then 0");

            await _containerClient.DeleteBlobIfExistsAsync(path, cancellationToken: token);

            using var memoryBuffer = new MemoryStream(data.ToArray());
            await _containerClient.UploadBlobAsync(path, memoryBuffer, token);
        }

        public async Task<IReadOnlyList<string>> Search(string prefix, Func<string, bool> filter, CancellationToken token)
        {
            var list = new List<string>();

            await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: token))
            {
                if (filter(blobItem.Name)) list.Add(blobItem.Name);
            }

            return list;
        }
    }
}
