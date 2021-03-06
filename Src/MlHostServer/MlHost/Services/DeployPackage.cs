﻿using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHost.Services
{
    internal class DeployPackage : IDeployPackage
    {
        private readonly IOption _option;
        private readonly IExecutionContext _executionContext;
        private readonly ILogger<DeployPackage> _logger;

        public DeployPackage(IOption option, IExecutionContext executionContext, ILogger<DeployPackage> logger)
        {
            _option = option;
            _executionContext = executionContext;
            _logger = logger;
        }

        public void Deploy()
        {
            string nameSpace = _option.PackageFile.ToNullIfEmpty() != null
                ? Path.GetFileNameWithoutExtension(_option.PackageFile)!
                : "runModel";

            string namespaceFolder = Assembly.GetExecutingAssembly().Location
                .Func(x => Path.GetDirectoryName(x)!)
                .Func(x => Path.Combine(x, nameSpace));

            ClearNamespace(namespaceFolder);

            _executionContext.DeploymentFolder = Path.Combine(namespaceFolder, $"MlPackageDeploy_{Guid.NewGuid()}");
            DeployPackageFromPackage();
        }

        private void ClearNamespace(string namespaceFolder)
        {
            Directory.CreateDirectory(namespaceFolder);
            string[] folders = Directory.GetDirectories(namespaceFolder);

            foreach (string folder in folders)
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex, $"Cannot delete folder {folder}");
                    // Swallow exception, clearing is just a best attempt
                }
            }
        }

        private void DeployPackageFromPackage()
        {
            int fileExtractedCount = 0;
            int totalFileCount = 0;

            using Timer timer = new Timer(
                x => _logger.LogInformation($"Extracting {fileExtractedCount} of {totalFileCount} files from package"),
                null,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5));

            try
            {
                switch (_option.PackageFile.ToNullIfEmpty() != null)
                {
                    case true:
                        _logger.LogInformation($"Deploying from {_option.PackageFile} to {_executionContext.DeploymentFolder}");
                        ZipArchiveTools.ExtractFromZipFile(
                            _option.PackageFile!,
                            _executionContext.DeploymentFolder!,
                            _executionContext.TokenSource.Token,
                            x =>
                            {
                                fileExtractedCount = x.Count;
                                totalFileCount = x.Total;
                            });
                        break;

                    default:
                        _logger.LogInformation($"Deploying from resource to {_executionContext.DeploymentFolder}");
                        ZipArchiveTools.ExtractZipFileFromResource(
                            typeof(MlHostedService),
                            "MlHost.MlPackage.RunModel.mlPackage",
                            _executionContext.DeploymentFolder!,
                            _executionContext.TokenSource.Token,
                            x =>
                            {
                                fileExtractedCount = x.Count;
                                totalFileCount = x.Total;
                            });
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to deploy package, ex={ex}");
                throw;
            }
        }
    }
}
