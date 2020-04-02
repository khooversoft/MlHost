using Microsoft.Extensions.Configuration;
using MlHostApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Repository;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostCli.Test.Application
{
    public class ModelFixture
    {
        private static ModelFixture? _current;
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        private const string _secretId = "MlHostCli.Test";

        private ModelFixture(IModelRepository modelRepository)
        {
            ModelRepository = modelRepository;
        }

        public IModelRepository ModelRepository { get; }

        public ITelemetry Telemetry { get; } = new FakeTelemetry();

        public async Task<IReadOnlyList<BlobInfo>> ListBlobs() => await _blobRepository.Search(string.Empty, x => true, CancellationToken.None);

        private async Task ClearAllBlob()
        {
            IReadOnlyList<BlobInfo> blobInfos = await ListBlobs();

            IReadOnlyList<(bool folder, BlobInfo blobInfo)> blobs = blobInfos
                .Select(x => x switch
                {
                    BlobInfo v when v.ContentLength == 0 && x.ContentType.ToNullIfEmpty() == null => (true, x),
                    _ => (false, x)
                })
                .ToList();

            // Delete files first
            await blobs
                .Where(x => !x.folder)
                .ForEachAsync(async x => await _blobRepository.Delete(x.blobInfo.Name!, CancellationToken.None));

            // then folders
            var folderList = blobs
                .Where(x => x.folder)
                .OrderByDescending(x => x.blobInfo.Name!.Split('/').Length)
                .ToList();

            foreach(var item in folderList)
            {
                await _blobRepository.Delete(item.blobInfo.Name!, CancellationToken.None);
            }
        }

        /// <summary>
        /// Global singleton constructor required because MS Test does not support test fixtures
        /// </summary>
        public static async Task<ModelFixture> GetModelFixture()
        {
            await _lock.WaitAsync();

            try
            {
                if (_current != null) return _current;

                IConfiguration config = new ConfigurationBuilder()
                    .AddUserSecrets(_secretId)
                    .AddEnvironmentVariables("mlhostcli")
                    .Build();

                var blobStoreOption = new BlobStoreOption();
                config.Bind(blobStoreOption, x => x.BindNonPublicProperties = true);
                blobStoreOption.Verify();

                //_current = new ModelFixture(blobRepository, modelRepository);

                await _current!.ClearAllBlob();
            }
            finally
            {
                _lock.Release();
            }

            return _current;
        }
    }
}
