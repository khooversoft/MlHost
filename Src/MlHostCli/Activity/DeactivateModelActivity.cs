using MlHostApi.Repository;
using MlHostCli.Application;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;

namespace MlHostCli.Activity
{
    internal class DeactivateModelActivity
    {
        private readonly IOption _option;
        private readonly IModelRepository _modelRepository;
        private readonly ITelemetry _telemetry;

        public DeactivateModelActivity(IOption option, IModelRepository modelRepository, ITelemetry telemetry)
        {
            _option = option;
            _modelRepository = modelRepository;
            _telemetry = telemetry;
        }

        public async Task Deactivate(CancellationToken token)
        {
            _telemetry.WriteLine($"Deactivating host {_option.HostName}");
            await _modelRepository.RemoveActivation(_option.HostName!, token);
        }
    }
}
