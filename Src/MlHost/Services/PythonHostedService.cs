using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Services
{
    public class PythonHostedService : IHostedService
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
                // Run in the background and set running state to run
                string deploymentFolder = await _packageDeployment.Deploy(cancellationToken);
                _executeTask = _executePython.Run(deploymentFolder);
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
