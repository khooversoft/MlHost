using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Models;
using MlHost.Tools;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHost.Services
{
    internal class MlHostedService : IHostedService
    {
        private readonly IOption _option;
        private readonly ILogger<MlHostedService> _logger;
        private readonly IExecuteModel _executeModel;
        private readonly IExecutionContext _executionContext;
        private readonly Activity[] _startupStrategy;
        private readonly Activity[] _restartStrategy;

        public MlHostedService(
            IOption option,
            ILogger<MlHostedService> logging,
            IExecuteModel executeModel,
            IExecutionContext executionContext,
            IDeployPackage deployPackage,
            ICleanupProcess cleanupProcess)
        {
            _option = option;
            _logger = logging;
            _executeModel = executeModel;
            _executionContext = executionContext;

            Task voidTask(Action x) { x(); return Task.CompletedTask; }

            _startupStrategy = new Activity?[]
            {
                new Activity("Resetting execution state to running", () => voidTask(() => _executionContext.State = ExecutionState.Starting)),
                new Activity("Deploy MlPackage to deployment folder", () => voidTask(() => deployPackage.Deploy())),
                _option.KillProcess ? new Activity("Kill running processes", () => voidTask(() => cleanupProcess.KillAnyRunningProcesses())) : null,
                new Activity("Start ML package", async () => await _executeModel.Start()),
            }
            .Where(x => x != null)
            .OfType<Activity>()
            .ToArray();

            _restartStrategy = _startupStrategy
                .Prepend(new Activity("Pausing...", async () => await Task.Delay(TimeSpan.FromSeconds(30))))
                .ToArray();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(_option.HostVersionTitle());
            _logger.LogInformation("Starting ML service");

            _ = Task.Run(() => Run());

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"ML service is stopping");

            _executionContext.TokenSource.Cancel();
            _executeModel.Stop();

            return Task.CompletedTask;
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
                    _logger.LogInformation($"ML service is running");
                    return;
                }
                catch
                {
                    activities = _restartStrategy;
                }
            }

            _executionContext.State = ExecutionState.Failed;
        }
    }
}