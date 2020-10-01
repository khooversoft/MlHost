using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Models;
using MlHostSdk.Package;
using MlHostSdk.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Services
{
    internal class CleanupProcess : ICleanupProcess
    {
        private readonly IExecutionContext _executionContext;
        private readonly ILogger<MlHostedService> _logger;

        public CleanupProcess(IExecutionContext executionContext, ILogger<MlHostedService> logger)
        {
            _executionContext = executionContext;
            _logger = logger;
        }

        public void KillAnyRunningProcesses()
        {
            var manifestFile = new MlPackageManifestFile(Path.Combine(_executionContext.DeploymentFolder!, MlPackageBuilder.ManifestFileName));

            string processName = Path.GetFileNameWithoutExtension(manifestFile.RunCmd);
            _logger.LogInformation($"Cleaning up processes for {processName}");

            foreach (var process in Process.GetProcessesByName(processName))
            {
                _logger.LogWarning($"Killing running process {process.ProcessName} before starting ML process");

                try
                {
                    process.Kill(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Cannot kill process {process.ProcessName}");
                    throw;
                }
            }
        }
    }
}
