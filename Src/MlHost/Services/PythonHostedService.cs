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
            IExecutionContext executionContext,
            IDeployPackage deployPackage)
        {
            _option = option;
            _logger = logging;
            _executePython = executePython;
            _executionContext = executionContext;

            Func<Action, Task> voidTask = x => { x(); return Task.CompletedTask; };

            _startupStrategy = new Activity?[]
            {
                new Activity("Resetting execution state to running", () => voidTask(() => _executionContext.State = ExecutionState.Starting)),
                _option.KillProcess ? new Activity("Kill running processes", () => voidTask(() => ProcessTools.KillAnyRunningProcesses(_logger))) : null,
                new Activity("Deploy MlPackage to deployment folder", () => voidTask(() => deployPackage.Deploy())),
                new Activity("Start ML package", async () => await _executePython.Start()),
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
            _logger.LogInformation("Starting python service");

            _ = Task.Run(() => Run());

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Python service is stopping");

            _executionContext.TokenSource.Cancel();
            _executePython.Stop();

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
    }
}