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
using Toolbox.Tools;

namespace MlHost.Services
{
    internal class PythonHostedService : IHostedService
    {
        private readonly IOption _option;
        private readonly ILogger<PythonHostedService> _logger;
        private readonly IPackageDeployment _packageDeployment;
        private readonly IExecutePython _executePython;
        private readonly IExecutionContext _executionContext;
        private readonly IModelRepository _modelRepository;
        private readonly IPackageSource _packageSource;
        private readonly IMlPackageService _mlPackageService;
        private readonly Activity[] _startupStrategy;
        private readonly Activity[] _restartStrategy;

        private Task? _executeTask;

        public PythonHostedService(IOption option,
            ILogger<PythonHostedService> logging,
            IPackageDeployment packageDeployment,
            IExecutePython executePython,
            IExecutionContext executionContext,
            IModelRepository modelRepository,
            IPackageSource packageSource,
            IMlPackageService mlPackageService)
        {
            _option = option;
            _logger = logging;
            _packageDeployment = packageDeployment;
            _executePython = executePython;
            _executionContext = executionContext;
            _modelRepository = modelRepository;
            _packageSource = packageSource;
            _mlPackageService = mlPackageService;

            _startupStrategy = new Activity[]
            {
                 new Activity("Get host registration", async () =>
                {
                    _executionContext.ModelId = await GetRegistration();
                    if( _executionContext.ModelId != null) return true;

                    _executionContext.State = ExecutionState.NoModelRegisteredForHost;
                    return false;
                }),
                
                new Activity("Starting ML Package", async () => await _mlPackageService.Start()),
            };

            _restartStrategy = _startupStrategy
                .Prepend(new Activity("Pausing...", async () => 
                {
                    await Task.Delay(TimeSpan.FromSeconds(30)); 
                    return true; 
                }))
                .ToArray();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting python service");

            _executionContext.ForceDeployment = _option.ForceDeployment;
            _ = Task.Run(() => Run());

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Python service is stopping");

            _executionContext.TokenSource.Cancel();

            return Interlocked.Exchange(ref _executeTask, null!) ?? Task.CompletedTask;
        }

        private async Task Run()
        {
            Activity[] activities = _startupStrategy;
            var range = new RangeLimit(0, 1);

            CancellationTokenSource tokenSource = CancellationTokenSource.CreateLinkedTokenSource(_executionContext.TokenSource.Token);

            while (!_executionContext.TokenSource.Token.IsCancellationRequested && range.Increment())
            {
                bool success = await activities.RunActivities(tokenSource.Token, _logger);
                if (success) return;

                _executionContext.ForceDeployment = true;

                switch (_executionContext.State)
                {
                    case ExecutionState.NoModelRegisteredForHost:

                        activities = _restartStrategy
                            .Prepend(new Activity("No ModelId assigned to this host, waiting...", async () => { await Task.Delay(TimeSpan.FromMinutes(5)); return true; }))
                            .ToArray();

                        range.Reset();
                        break;

                    default:
                        activities = _restartStrategy;
                        break;
                }
            }

            _executionContext.State = ExecutionState.Failed;
        }

        private async Task<ModelId?> GetRegistration()
        {
            _logger.LogInformation($"Retrieving registration for host {_option.HostName}");
            ModelId? modelId = await _modelRepository.GetRegistration(_option.HostName!, _executionContext.TokenSource.Token);

            Action info = () => _logger.LogInformation($"Retrieved registration, host {_option.HostName} is assigned model {modelId}");
            Action error = () => _logger.LogError($"Failed to retrieved registration for host {_option.HostName}");

            (modelId != null ? info : error)();

            return modelId;
        }
    }
}
