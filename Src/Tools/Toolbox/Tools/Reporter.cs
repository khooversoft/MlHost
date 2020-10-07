using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace Toolbox.Tools
{
    public class Reporter : IProgress<long>, IDisposable
    {
        private readonly string _label;
        private readonly ILogger _logger;
        private readonly Func<long, string> _formatter;
        private Timer _timer;
        private long _value;

        public Reporter(string label, Func<long, string> formatter, ILogger logger)
        {

            label.VerifyNotEmpty(nameof(label));
            formatter.VerifyNotNull(nameof(formatter));
            logger.VerifyNotNull(nameof(logger));

            _label = label;
            _formatter = formatter;
            _logger = logger;

            _timer = new Timer(x => ReportValue(), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        public long Value => _value;

        public void Dispose() => Interlocked.Exchange(ref _timer, null!)?.Dispose();

        void IProgress<long>.Report(long value) => SetValue(value);

        public void SetValue(long count) => Interlocked.Exchange(ref _value, count);

        private void ReportValue() => _logger.LogInformation($"{_label}: Progress={_formatter(_value)}");
    }
}
