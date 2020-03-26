using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHostApi.Repository;
using MlHostApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        private Task? _executeTask;

        public PythonHostedService(IOption option, ILogger<PythonHostedService> logging, IPackageDeployment packageDeployment, IExecutePython executePython, IExecutionContext executionContext, IModelRepository modelRepository)
        {
            _option = option;
            _logging = logging;
            _packageDeployment = packageDeployment;
            _executePython = executePython;
            _executionContext = executionContext;
            _modelRepository = modelRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logging.LogInformation("Starting python service");

            _ = Task.Run(async () =>
            {
                _executionContext.State = ExecutionState.Starting;

                // Run in the background and set running state to run when completed
                _executePython.KillAnyRunningProcesses();

                _executionContext.ModelId = await GetRegistration();
                if (_executionContext.ModelId == null) return;

                await _packageDeployment.Deploy(_executionContext.ModelId);
                _executeTask = _executePython.Run();
            }, cancellationToken);

            return Task.CompletedTask;
        }

        private async Task<ModelId?> GetRegistration()
        {
            while(!_executionContext.TokenSource.Token.IsCancellationRequested)
            {
                _logging.LogInformation($"Retrieving registration for host {_option.HostName}");
                ModelId? modelId = await _modelRepository.GetRegistration(_option.HostName!, _executionContext.TokenSource.Token);
                if (modelId != null)
                {
                    _logging.LogInformation($"Retrieved registration, host {_option.HostName} is assigned model {modelId}");
                    return modelId;
                }

                await Task.Delay(TimeSpan.FromMinutes(5));
            }

            return null;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logging.LogInformation($"Python service is stopping");

            _executionContext.TokenSource.Cancel();

            return Interlocked.Exchange(ref _executeTask, null!) ?? Task.CompletedTask;
        }
    }
}
