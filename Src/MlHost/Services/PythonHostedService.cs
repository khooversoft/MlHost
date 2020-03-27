using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Tools;
using MlHostApi.Repository;
using MlHostApi.Tools;
using MlHostApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MlHost.Services
{
    internal class PythonHostedService : IHostedService
    {
        private readonly IOption _option;
        private readonly ILogger<PythonHostedService> _logging;
        private readonly IPackageDeployment _packageDeployment;
        private readonly IExecutePython _executePython;
        private readonly IExecutionContext _executionContext;
        private readonly IModelRepository _modelRepository;
        private readonly IPackageSource _packageSource;

        private readonly Activity[] _startupActivities;
        private readonly Activity[] _restartActivities;

        private Task? _executeTask;

        public PythonHostedService(IOption option,
            ILogger<PythonHostedService> logging,
            IPackageDeployment packageDeployment,
            IExecutePython executePython,
            IExecutionContext executionContext,
            IModelRepository modelRepository,
            IPackageSource packageSource)
        {
            _option = option;
            _logging = logging;
            _packageDeployment = packageDeployment;
            _executePython = executePython;
            _executionContext = executionContext;
            _modelRepository = modelRepository;
            _packageSource = packageSource;

            _startupActivities = new Activity[]
            {
                new Activity("Resetting execution state to running", () => { _executionContext.State = ExecutionState.Starting; return Task.FromResult(true); }),
                new Activity("Kill running processes", () => _executePython.KillAnyRunningProcesses()),
                new Activity("Get host registration", async () =>
                {
                    _executionContext.ModelId = await GetRegistration();
                    if( _executionContext.ModelId != null) return true;

                    _executionContext.State = ExecutionState.NoModelRegisteredForHost;
                    return false;
                }),
                new Activity("Download package if required", async () => await _packageSource.GetPackageIfRequired(_executionContext.ForceDeployment)),
                new Activity("Deploy ML package", async () =>
                {
                    await _packageDeployment.Deploy();
                    return true;
                }),
                new Activity("Start ML package", () =>
                {
                    _executeTask = _executePython.Run();
                    return Task.FromResult(true);
                })
            };

            _restartActivities = _startupActivities
                .Prepend(new Activity("Pausing...", async () => { await Task.Delay(TimeSpan.FromSeconds(3)); return true; }))
                .ToArray();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logging.LogInformation("Starting python service");

            _executionContext.ForceDeployment = _option.ForceDeployment;
            _ = Task.Run(() => Startup());

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logging.LogInformation($"Python service is stopping");

            _executionContext.TokenSource.Cancel();

            return Interlocked.Exchange(ref _executeTask, null!) ?? Task.CompletedTask;
        }

        private async Task Startup()
        {
            Activity[] activities = _startupActivities;
            var range = new RangeLimit(0, 1);

            while (!_executionContext.TokenSource.Token.IsCancellationRequested && range.Increment())
            {
                bool success = await RunActivities(activities);
                if (success) return;

                _executionContext.ForceDeployment = true;

                switch (_executionContext.State)
                {
                    case ExecutionState.NoModelRegisteredForHost:

                        activities = _restartActivities
                            .Prepend(new Activity("No ModelId assigned to this host, waiting...", async () => { await Task.Delay(TimeSpan.FromSeconds(3)); return true; }))
                            .ToArray();

                        range.Reset();
                        break;

                    default:
                        activities = _restartActivities;
                        break;
                }
            }

            _executionContext.State = ExecutionState.Failed;
        }

        private async Task<ModelId?> GetRegistration()
        {
            _logging.LogInformation($"Retrieving registration for host {_option.HostName}");
            ModelId? modelId = await _modelRepository.GetRegistration(_option.HostName!, _executionContext.TokenSource.Token);

            Action info = () => _logging.LogInformation($"Retrieved registration, host {_option.HostName} is assigned model {modelId}");
            Action error = () => _logging.LogError($"Failed to retrieved registration for host {_option.HostName}");

            (modelId != null ? info : error)();

            return modelId;
        }

        private async Task<bool> RunActivities(IEnumerable<Activity> activities)
        {
            foreach (var activity in activities)
            {
                if (!(await Run(activity))) return false;
            }

            return true;

            async Task<bool> Run(Activity activity)
            {
                try
                {
                    _logging.LogInformation($"Starting activity {activity.Description}");
                    bool success = await activity.Func();

                    _logging.LogInformation($"Completed activity {activity.Description}, status={success}");
                    return success;
                }
                catch (Exception ex)
                {
                    _logging.LogError(ex, $"Activity {activity.Description} failed");
                    return false;
                }
            }
        }

        private class Activity
        {
            public Activity(string description, Func<Task<bool>> func)
            {
                Description = description;
                Func = func;
            }

            public string Description { get; }

            public Func<Task<bool>> Func { get; }
        }
    }
}
