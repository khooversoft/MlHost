using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MlHost.Application;
using MlHost.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MlHost.Services.AzureBlob
{
    internal class BlobRepository : IBlobRepository
    {
        private readonly IExecutionContext _executionContext;
        private readonly IOption _option;
        private readonly BlobContainerClient _containerClient;

        public BlobRepository(IExecutionContext executionContext, IOption option)
        {
            _executionContext = executionContext;
            _option = option;

            var client = new BlobServiceClient(_option.BlobStore.ConnectionString);
            _containerClient = client.GetBlobContainerClient(_option.BlobStore.ContainerName);
        }

        public async Task Download(string path, Stream toStream)
        {
            path = path.VerifyNotEmpty(nameof(path));

            BlobClient blobClient = _containerClient.GetBlobClient(path);
            BlobDownloadInfo download = await blobClient.DownloadAsync(_executionContext.TokenSource.Token);

            await download.Content.CopyToAsync(toStream);
        }
    }
}
