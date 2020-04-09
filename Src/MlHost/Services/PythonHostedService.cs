using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Tools;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHost.Services
{
    internal class PythonHostedService : IHostedService
    {
        private readonly IOption _option;
        private readonly ILogger<PythonHostedService> _logger;
        private readonly IExecutePython _executePython;
        private readonly IExecutionContext _executionContext;
        private readonly Activity[] _startupStrategy;
        private readonly Activity[] _restartStrategy;

        public PythonHostedService(
            IOption option,
            ILogger<PythonHostedService> logging,
            IExecutePython executePython,
            IExecutionContext executionContext)
        {
            _option = option;
            _logger = logging;
            _executePython = executePython;
            _executionContext = executionContext;

            Func<Action, Task> voidTask = x => { x(); return Task.CompletedTask; };

            _startupStrategy = new Activity[]
            {
                new Activity("Resetting execution state to running", () => voidTask(() => _executionContext.State = ExecutionState.Starting)),
                new Activity("Kill running processes", () => voidTask(() => ProcessTools.KillAnyRunningProcesses(_logger))),
                new Activity("Deploy MlPackage to deployment folder", () => voidTask(() => DeployPackage())),
                new Activity("Start ML package", async () => await _executePython.Run()),
            };

            _restartStrategy = _startupStrategy
                .Prepend(new Activity("Pausing...", async () => await Task.Delay(TimeSpan.FromSeconds(30))))
                .ToArray();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting python service");

            _ = Task.Run(() => Run());

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Python service is stopping");

            _executionContext.TokenSource.Cancel();
            await _executePython.Stop();
        }

        private async Task Run()
        {
            Activity[] activities = _startupStrategy;
            var range = new RangeLimit(0, 1);

            while (!_executionContext.TokenSource.Token.IsCancellationRequested && range.Increment())
            {
                try
                {
                    await activities.RunActivities(_executionContext.TokenSource.Token, _logger);
                    _logger.LogInformation($"Python service is running");
                    return;
                }
                catch
                {
                    activities = _restartStrategy;
                }
            }

            _executionContext.State = ExecutionState.Failed;
        }

        private void DeployPackage()
        {
            bool isPackageRequested = _option.PackageFile.ToNullIfEmpty() != null;

            switch(isPackageRequested)
            {
                case true:
                    _logger.LogInformation($"Deploying from {_option.PackageFile} to {_option.DeploymentFolder}");
                    ZipArchiveTools.ExtractFromZipFile(_option.PackageFile!, _option.DeploymentFolder, _executionContext.TokenSource.Token);
                    break;

                default:
                    _logger.LogInformation($"Deploying from resource to {_option.DeploymentFolder}");
                    ZipArchiveTools.ExtractZipFileFromResource(typeof(PythonHostedService), "MlHost.MlPackage.RunModel.mlPackage", _option.DeploymentFolder, _executionContext.TokenSource.Token);
                    break;
            }
        }
    }
}