using MlHostCli.Application;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostCli.Activity
{
    internal class DumpConfigurationActivity
    {
        private readonly IOption _option;
        private readonly ITelemetry _telemetry;

        public DumpConfigurationActivity(IOption option, ITelemetry telemetry)
        {
            _option = option;
            _telemetry = telemetry;
        }

        public Task Dump()
        {
            const int maxWidth = 80;

            _option.GetConfigValues()
                .Select(x => "  " + x)
                .Prepend(new string('=', maxWidth))
                .Prepend("Current configurations")
                .Append(string.Empty)
                .Append(string.Empty)
                .ForEach(x => _telemetry.WriteLine(x));

            return Task.CompletedTask;
        }
    }
}
