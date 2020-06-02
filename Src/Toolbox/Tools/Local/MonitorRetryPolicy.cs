using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Toolbox.Tools.Local
{
    public class MonitorRetryPolicy
    {
        private readonly MonitorRetryPolicyConfig _monitorRetryPolicyConfig;
        private object _lock = new object();

        private int _retryCount;
        private DateTime? _lastRetryDate;

        public MonitorRetryPolicy(MonitorRetryPolicyConfig monitorRetryPolicyConfig)
        {
            monitorRetryPolicyConfig.VerifyNotNull(nameof(monitorRetryPolicyConfig));

            _monitorRetryPolicyConfig = monitorRetryPolicyConfig;
        }

        public int RetryCount => _retryCount;

        public bool CanRetry()
        {
            lock (_lock)
            {
                if (_lastRetryDate != null && _monitorRetryPolicyConfig.WithIn != null)
                {
                    if (DateTime.Now - _lastRetryDate > (TimeSpan)_monitorRetryPolicyConfig.WithIn) _retryCount = 0;
                }
            }

            int retryCount = Interlocked.Increment(ref _retryCount);
            _lastRetryDate = DateTime.Now;

            return retryCount < _monitorRetryPolicyConfig.MaxRetry;
        }
    }
}
