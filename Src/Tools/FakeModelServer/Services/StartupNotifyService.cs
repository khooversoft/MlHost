using FakeModelServer.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FakeModelServer.Services
{
    public class StartupNotifyService : BackgroundService
    {
        private readonly IOption _option;
        private readonly ILogger<StartupNotifyService> _logging;

        public StartupNotifyService(IOption option, ILogger<StartupNotifyService> logging)
        {
            _option = option;
            _logging = logging;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            _logging.LogInformation($"Running on port {_option.Port}");
        }
    }
}
