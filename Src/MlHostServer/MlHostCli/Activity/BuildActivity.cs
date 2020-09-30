using Microsoft.Extensions.Logging;
using MlHostCli.Application;
using MlHostSdk.Package;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;

namespace MlHostCli.Activity
{
    internal class BuildActivity
    {
        private readonly IOption _option;
        private readonly ILogger<BuildActivity> _logger;

        public BuildActivity(IOption option, ILogger<BuildActivity> logger)
        {
            _option = option;
            _logger = logger;
        }

        public Task Build(CancellationToken token)
        {
            _logger.LogInformation($"Building ML Package defined by {_option.SpecFile}");

            int totalFileCount = new MlPackageBuilder()
                .ReadOption(_option.SpecFile!)
                .Build(x =>
                {
                    if (x.Count % 1000 == 0) _logger.LogInformation($"Processed {x.Count} files");
                });

            _logger.LogInformation($"Processed {_option.SpecFile} file, total files {totalFileCount}");
            return Task.CompletedTask;
        }
    }
}
