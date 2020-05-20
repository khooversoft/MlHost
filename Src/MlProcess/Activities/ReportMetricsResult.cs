using Microsoft.Extensions.Logging;
using MlProcess.Application;
using MlProcess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlProcess.Activities
{
    internal class ReportMetricsResult
    {
        private readonly IMetricSampler _metricSampler;
        private readonly ILogger<ReportMetricsResult> _logger;

        public ReportMetricsResult(IMetricSampler metricSampler, ILogger<ReportMetricsResult> logger)
        {
            _metricSampler = metricSampler;
            _logger = logger;
        }

        public Task Report()
        {
            _logger.LogInformation($"{nameof(ReportMetricsResult)}: Model's metric results");

            _metricSampler.GetSamples()
                .ForEach(x => _logger.LogInformation($"Totals: Model={x.Name,-20}, Total: {x.Count,10}, Span: {x.Span:dd\\.hh\\:mm\\:ss}, TPS: {x.Count / x.Span.TotalSeconds:N4}"));

            return Task.CompletedTask;
        }
    }
}
