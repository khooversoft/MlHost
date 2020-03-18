using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Services
{
    internal class PythonHostedService : IHostedService
    {
        private readonly ILogger<PythonHostedService> _logging;
        private readonly IPackageDeployment _packageDeployment;
        private readonly IExecutePython _executePython;
        private readonly IExecutionContext _executionContext;
        private Task? _executeTask;

        public PythonHostedService(ILogger<PythonHostedService> logging, IPackageDeployment packageDeployment, IExecutePython executePython, IExecutionContext executionContext)
        {
            _logging = logging;
            _packageDeployment = packageDeployment;
            _executePython = executePython;
            _executionContext = executionContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logging.LogInformation("Starting python service");

            _ = Task.Run(async () =>
            {
                // Run in the background and set running state to run when completed
                _executePython.KillAnyRunningProcesses();

                await _packageDeployment.Deploy();
                _executeTask = _executePython.Run();
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logging.LogInformation($"Python service is stopping");

            _executionContext.TokenSource.Cancel();

            return Interlocked.Exchange(ref _executeTask, null!) ?? Task.CompletedTask;
        }
    }
}
