﻿using Microsoft.Extensions.Logging;
using MlHostSdk.Repository;
using MlHostSdk.Types;
using MlHostCli.Application;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;

namespace MlHostCli.Activity
{
    internal class DeleteModelActivity
    {
        private readonly IOption _option;
        private readonly IModelStore _modelRepository;
        private readonly ILogger<DeleteModelActivity> _logger;

        public DeleteModelActivity(IOption option, IModelStore modelRepository, ILogger<DeleteModelActivity> logger)
        {
            _option = option;
            _modelRepository = modelRepository;
            _logger = logger;
        }

        public async Task Delete(CancellationToken token)
        {
            var modelId = new ModelId(_option.ModelName!, _option.VersionId!);

            _logger.LogInformation($"Deleting model {modelId}");
            bool deleted = await _modelRepository.Delete(modelId, token);

            if (!deleted) _logger.LogInformation($"The {modelId} does not exist");
        }
    }
}
