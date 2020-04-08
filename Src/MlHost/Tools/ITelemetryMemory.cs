using System.Collections.Generic;

namespace MlHost.Tools
{
    public interface ITelemetryMemory
    {
        void Add(string message);

        IReadOnlyList<string> GetLoggedMessages();
    }
}