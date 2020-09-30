using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Toolbox.Application
{
    public class MemoryLogger<T> : MemoryLogger, ILogger<T>
    {
    }

    public class MemoryLogger : ILogger, IEnumerable<string>
    {
        private readonly ConcurrentQueue<string> _loggingQueue = new ConcurrentQueue<string>();

        public MemoryLogger() { }

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        public bool IsEnabled(LogLevel logLevel) => throw new NotImplementedException();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _loggingQueue.Enqueue(formatter(state, exception));
            Debug.WriteLine($"{nameof(Log)} - {formatter(state, exception)}");
        }

        public IEnumerator<string> GetEnumerator() => _loggingQueue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _loggingQueue.GetEnumerator();
    }
}
