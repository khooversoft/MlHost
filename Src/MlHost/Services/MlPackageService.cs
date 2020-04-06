using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Tools;
using MlHostApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHost.Services
{
    internal class MlPackageService : IMlPackageService
    {
        private readonly IOption _option;
        private readonly ILogger<MlPackageService> _logger;
        private readonly IPackageDeployment _packageDeployment;
        private readonly IExecutePython _executePython;
        private readonly IExecutionContext _executionContext;
        private readonly IPackageSource _packageSource;
        private readonly Activity[] _activities;

        private CancellationTokenSource? _tokenSource;

        private Task? _executeTask;


        public MlPackageService(IOption option,
            ILogger<MlPackageService> logger,
            IPackageDeployment packageDeployment,
            IExecutePython executePython,
            IExecutionContext executionContext,
            IPackageSource packageSource)
        {
            _option = option;
            _logger = logger;
            _packageDeployment = packageDeployment;
            _executePython = executePython;
            _executionContext = executionContext;
            _packageSource = packageSource;

            Func<Action, Task<bool>> voidActivity = x => { x(); return Task.FromResult(true); };

            _activities = new Activity[]
            {
                new Activity("Resetting execution state to running", () => voidActivity(() => _executionContext.State = ExecutionState.Starting)),

                new Activity("Kill running processes", () => voidActivity(() => ProcessTools.KillAnyRunningProcesses(_logger))),

                new Activity("Download package if required", async () => await _packageSource.GetPackageIfRequired(_executionContext.ForceDeployment)),

                new Activity("Deploy ML package", () => voidActivity(async () => await _packageDeployment.Deploy())),

                new Activity("Start ML package", () => voidActivity(() => _executeTask = _executePython.Run(_tokenSource!.Token))),
            };
        }

        public async Task<bool> Start()
        {
            _tokenSource = new CancellationTokenSource();
            _executeTask = null;

            return await _activities.RunActivities(_tokenSource.Token, _logger);
        }

        public Task Stop()
        {
            _logger.LogInformation($"Python service is stopping");

            _tokenSource?.Cancel();

            return Interlocked.Exchange(ref _executeTask, null!) ?? Task.CompletedTask;
        }
    }
}
