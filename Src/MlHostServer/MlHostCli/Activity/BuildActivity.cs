using Microsoft.Extensions.Logging;
using MlHostCli.Application;
using MlHostSdk.Package;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

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

            BuildResults results;

            using (var reporting = new Reporter(nameof(Build), x => $"{x:#,###} count", _logger))
            {
                results = new MlPackageBuilder()
                    .ReadSpecFile(_option.SpecFile!)
                    .Build(x => reporting.SetValue(x.Count), token);
            }

            _logger.LogInformation($"Completed processed of {_option.SpecFile}");
            _logger.LogInformation($"Processed {results.Option.PackageFile} file, total files {results.FileCount}");

            return Task.CompletedTask;
        }
    }
}
