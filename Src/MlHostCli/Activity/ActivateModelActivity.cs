using MlHostApi.Repository;
using MlHostApi.Types;
using MlHostCli.Application;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostCli.Activity
{
    internal class ActivateModelActivity
    {
        private readonly IOption _option;
        private readonly IModelRepository _modelRepository;
        private readonly ITelemetry _telemetry;

        public ActivateModelActivity(IOption option, IModelRepository modelRepository, ITelemetry telemetry)
        {
            _option = option;
            _modelRepository = modelRepository;
            _telemetry = telemetry;
        }

        public async Task Activate(CancellationToken token)
        {
            var modelId = new ModelId(_option.ModelName!, _option.VersionId!);

            (await _modelRepository.Exist(modelId, token)).VerifyAssert(x => x == true, $"Model {modelId} does not exist to activate");

            _telemetry.WriteLine($"Activating model {modelId} for host {_option.HostName}");
            await _modelRepository.AddActivation(_option.HostName!, modelId, token);
        }
    }
}
