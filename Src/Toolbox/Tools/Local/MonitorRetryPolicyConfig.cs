using System;

namespace Toolbox.Tools.Local
{
    public class MonitorRetryPolicyConfig
    {
        public MonitorRetryPolicyConfig(int maxRetry, TimeSpan? withIn)
        {
            maxRetry.VerifyAssert(x => x > 0, "Max retry must be greater then 0");

            MaxRetry = maxRetry;
            WithIn = withIn;
        }

        public int MaxRetry { get; }

        public TimeSpan? WithIn { get; }
    }
}
