using Microsoft.Extensions.Logging;
using MlProcess.Application;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlProcess.Services
{
    internal class MetricSampler : IMetricSampler
    {
        private ConcurrentDictionary<string, int> _nameCount = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private readonly IOption _option;
        private readonly ILogger<MetricSampler> _logger;
        private readonly object _lock = new object();
        private bool _isRunning = false;
        private DateTime _startTime;
        private DateTime? _stopTime;
        private Timer? _timer;

        public MetricSampler(IOption option, ILogger<MetricSampler> logger)
        {
            _option = option;
            _logger = logger;
        }

        public void Start()
        {
            Stop();

            _isRunning = true;
            _nameCount.Clear();
            _startTime = DateTime.Now;
            _stopTime = null;
            _timer = new Timer(x => Report(), null, TimeSpan.FromSeconds(_option.SampleRate), TimeSpan.FromSeconds(_option.SampleRate));
        }

        public void Add(string name)
        {
            name.VerifyNotEmpty(nameof(name));

            lock (_lock)
            {
                _isRunning.VerifyAssert(x => x == true, "Sampler is not running");
                _nameCount.AddOrUpdate(name, 1, (k, x) => x + 1);
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                _stopTime = DateTime.Now;
                _isRunning = false;
                Interlocked.Exchange(ref _timer, null!)?.Dispose();
            }
        }

        public IReadOnlyList<MetricSample> GetSamples()
        {
            lock (_lock)
            {
                TimeSpan span = (_stopTime ?? DateTime.Now) - _startTime;

                return _nameCount
                    .Select(x => new MetricSample(x.Key, span, x.Value))
                    .ToList();
            }
        }

        private void Report()
        {
            TimeSpan span = DateTime.Now - _startTime;

            _nameCount
                .ForEach(x => _logger.LogInformation($"Model:{x.Key,-20}, Count:{x.Value,10}, Span:{span:hh\\:mm\\:ss}, TPS: {x.Value / span.TotalSeconds:N4}"));
        }

        public void Dispose() => Stop();
    }
}
