using MlHostApi.Models;
using MlHostApi.Repository;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostCli.Activity
{
    internal class ListModelActivity
    {
        private readonly IModelRepository _modelRepository;
        private readonly ITelemetry _telemetry;

        public ListModelActivity(IModelRepository modelRepository, ITelemetry telemetry)
        {
            _modelRepository = modelRepository;
            _telemetry = telemetry;
        }

        public async Task List(CancellationToken token)
        {
            HostConfigurationModel model = await _modelRepository.ReadConfiguration(token);

            const string fmt = "{0,-20} {1}";

            _telemetry.WriteLine("");
            _telemetry.WriteLine(string.Format(fmt, "Host name", "Model ID"));
            _telemetry.WriteLine(string.Format(fmt, new string('=', 20), new string('=', 50)));

            model.HostAssignments
                ?.ForEach(x => _telemetry.WriteLine(string.Format(fmt, x.HostName, x.ModelId)));
        }
    }
}
