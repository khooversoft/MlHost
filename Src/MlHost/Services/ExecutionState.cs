using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Services
{
    public enum ExecutionState
    {
        Booting,
        Starting,
        Deploying,
        Running,
        Failed,
        NoModelRegisteredForHost
    }
}
