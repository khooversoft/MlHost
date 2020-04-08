using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHost.Tools
{
    internal class TelemetryMemoryLogger : ILogger
    {
        private readonly string _category;
        private readonly ITelemetryMemory _memoryTelemetry;

        public TelemetryMemoryLogger(string category, ITelemetryMemory telemetryMemory)
        {
            category.VerifyNotEmpty(nameof(category));
            telemetryMemory.VerifyNotNull(nameof(telemetryMemory));

            _category = category;
            _memoryTelemetry = telemetryMemory;
        }

        public IDisposable BeginScope<TState>(TState state) => null!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _memoryTelemetry.Add($"[{_category}] " + formatter(state, exception));
        }
    }
}
