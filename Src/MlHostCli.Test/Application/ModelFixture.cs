using Microsoft.Extensions.Configuration;
using MlHostApi.Repository;
using MlHostApi.Services;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MlHostCli.Test.Application
{
    public class ModelFixture : IDisposable
    {
        private const string _secretId = "MlHostCli.Test";
        private readonly IBlobRepository _blobRepository;
        private int _disposed = 0;

        public ModelFixture()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddUserSecrets(_secretId)
                .Build();

            Func<string, string> getValue = x => config[x].VerifyNotEmpty($"{x} is required");

            string containerName = "BlobStore:ContainerName".Func(x => getValue(x));
            string connectionString = "BlobStore:ConnectionString".Func(x => getValue(x));

            _blobRepository = new BlobRepository(containerName, connectionString);
            ModelRepository = new ModelRepository(_blobRepository, new Json());

            ClearAllBlob().Wait(TimeSpan.FromSeconds(10));
        }

        public IModelRepository ModelRepository { get; }


        public async Task<IReadOnlyList<string>> ListBlobs() => await _blobRepository.Search(string.Empty, x => true, CancellationToken.None);

        public void Dispose()
        {
            int disposed = Interlocked.Exchange(ref _disposed, 1);
            if( disposed == 0)
            {
                ClearAllBlob().Wait();
            }
        }

        private async Task ClearAllBlob()
        {
            IReadOnlyList<string> blobUris = await ListBlobs();

            await blobUris
                .ForEachAsync(async x => await _blobRepository.Delete(x, CancellationToken.None));
        }
    }
}
