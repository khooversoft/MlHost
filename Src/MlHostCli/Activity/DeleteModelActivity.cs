using Microsoft.Extensions.Logging;
using MlHostApi.Repository;
using MlHostApi.Types;
using MlHostCli.Application;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;

namespace MlHostCli.Activity
{
    internal class DeleteModelActivity
    {
        private readonly IOption _option;
        private readonly IModelRepository _modelRepository;
        private readonly ILogger<DeleteModelActivity> _logger;

        public DeleteModelActivity(IOption option, IModelRepository modelRepository, ILogger<DeleteModelActivity> logger)
        {
            _option = option;
            _modelRepository = modelRepository;
            _logger = logger;
        }

        public Task Delete(CancellationToken token)
        {
            var modelId = new ModelId(_option.ModelName!, _option.VersionId!);

            _logger.LogInformation($"Deleting model {modelId}");
            return _modelRepository.Delete(modelId, token);
        }
    }
}
