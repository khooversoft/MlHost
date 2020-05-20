using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Services;

namespace Toolbox.Tools
{
    public class RetryPolicy
    {
        private static readonly Random _random = new Random();
        private readonly int _minMs;
        private readonly int _maxMs;
        private readonly TimeSpan _timeout;
        private readonly ILogger? _logger;

        public RetryPolicy(int minMs, int maxMs, TimeSpan timeout, ILogger? logger = null)
        {
            minMs.VerifyAssert(x => x < maxMs, $"{nameof(minMs)} is not less then {nameof(maxMs)}");
            timeout.VerifyAssert(x => x.TotalMilliseconds > 0, $"{nameof(timeout)} must be greater then 0 milliseconds");

            _minMs = minMs;
            _maxMs = maxMs;
            _timeout = timeout;
            _logger = logger;
        }

        public async Task<T> Invoke<T>(string name, Func<Task<T>> action)
        {
            var start = DateTime.Now;
            int retryCount = 0;

            while (DateTime.Now - start < _timeout)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"ERROR: exception encountered for {name}, {ex}, retrying after delay");

                    retryCount++;
                    int waitMs = _random.Next(_minMs, _maxMs);
                    _logger?.LogError($"Retry {retryCount} for {name}, waiting ms {waitMs}, timeout:{_timeout}, time left:{DateTime.Now - start}");

                    await Task.Delay(TimeSpan.FromMilliseconds(waitMs));
                }
            }

            throw new TimeoutException("Call to ML host retry policy timed out");
        }
    }
}
