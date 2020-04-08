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
        private readonly ILogger<PythonHostedService> _logger;
        private readonly IExecutePython _executePython;
        private readonly IExecutionContext _executionContext;
        private readonly ITelemetryMemory _telemetryMemory;
        private readonly Activity[] _startupStrategy;
        private readonly Activity[] _restartStrategy;

        public PythonHostedService(
            ILogger<PythonHostedService> logging,
            IExecutePython executePython,
            IExecutionContext executionContext, ITelemetryMemory telemetryMemory)
        {
            _logger = logging;
            _executePython = executePython;
            _executionContext = executionContext;
            _telemetryMemory = telemetryMemory;

            Func<Action, Task> voidTask = x => { x(); return Task.CompletedTask; };

            _startupStrategy = new Activity[]
            {
                new Activity("Resetting execution state to running", () => voidTask(() => _executionContext.State = ExecutionState.Starting)),
                new Activity("Kill running processes", () => voidTask(() => ProcessTools.KillAnyRunningProcesses(_logger))),
                new Activity("Start ML package", async () => await _executePython.Run()),
            };

            _restartStrategy = _startupStrategy
                .Prepend(new Activity("Pausing...", async () => await Task.Delay(TimeSpan.FromSeconds(30))))
                .ToArray();

            _telemetryMemory.Add($"([{nameof(PythonHostedService)}] constructed");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _telemetryMemory.Add($"([{nameof(PythonHostedService)})] starting");
            _logger.LogInformation("Starting python service");

            _ = Task.Run(() => Run());

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _telemetryMemory.Add($"([{nameof(PythonHostedService)})] stopping");
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

                    _telemetryMemory.Add($"([{nameof(PythonHostedService)}] Running");
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