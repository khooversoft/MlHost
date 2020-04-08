﻿using Microsoft.Extensions.Configuration;
using MlHostApi.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Models;
using Toolbox.Repository;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostCli.Test.Application
{
    public class ModelFixture
    {
        internal const string _resourceId = "MlHostCli.Test.Application.TestConfig.json";
        private static ModelFixture? _current;
        private static object _lock = new object();

        private const string _secretId = "MlHostCli.Test";

        private ModelFixture(IModelRepository modelRepository)
        {
            ModelRepository = modelRepository;
        }

        public IModelRepository ModelRepository { get; }

        public ITelemetry Telemetry { get; } = new FakeTelemetry();

        public async Task<IReadOnlyList<DatalakePathItem>> ListFiles() => await ModelRepository.Search(null, "*", CancellationToken.None);

        /// <summary>
        /// Global singleton constructor required because MS Test does not support test fixtures
        /// </summary>
        public static ModelFixture GetModelFixture()
        {
            lock(_lock)
            {
                if (_current != null) return _current;

                using Stream configStream = typeof(TestOption).GetResourceStream(_resourceId);

                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonStream(configStream)
                    .AddUserSecrets(_secretId)
                    .AddEnvironmentVariables("mlhostcli")
                    .Build();

                var blobStoreOption = new StoreOption();
                config.Bind(blobStoreOption, x => x.BindNonPublicProperties = true);
                blobStoreOption.Verify();

                return _current = new ModelFixture(new ModelRepository(new DatalakeRepository(blobStoreOption), new Json()));
            }
        }
    }
}
