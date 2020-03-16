using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Services
{
    public class PackageDeployment : IPackageDeployment
    {
        private const string _deployFolderName = "MlHostStorageDeploy";
        private const string _resourcePath = "MlHost.Package.ml-package.zip";

        private readonly ILogger<PackageDeployment> _logger;
        private readonly IOption _option;

        public PackageDeployment(ILogger<PackageDeployment> logger, IOption option)
        {
            _logger = logger;
            _option = option;
        }

        public Task<string> Deploy(CancellationToken token)
        {
            string executingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string deploymentFolder = Path.Combine(executingFolder, _deployFolderName);

            bool folderExist = Directory.Exists(deploymentFolder);

            _logger.LogInformation($"Deploying ML code & model, deployment folder={deploymentFolder}, exist={folderExist}, forceDelete={_option.ForceDeployment}");
            if (folderExist && _option.ForceDeployment) ResetDeploymentFolder(deploymentFolder);

            new PackageUpdate(_logger, token).UpdateToFolder(deploymentFolder, typeof(PackageDeployment), _resourcePath);
            _logger.LogInformation($"Deployed ML code & model");

            return Task.FromResult(deploymentFolder);
        }

        private void ResetDeploymentFolder(string deploymentFolder)
        {
            _logger.LogInformation($"Deleting exiting deployment folder {deploymentFolder}");
            Directory.Delete(deploymentFolder, true);

            var startTime = DateTime.Now;
            var limit = TimeSpan.FromSeconds(10);
            while (Directory.Exists(deploymentFolder) && DateTime.Now - startTime < limit)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }
        }
    }
}
