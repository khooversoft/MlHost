using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Services
{
    public class ExecutionContext : IExecutionContext
    {
        public ExecutionContext() { }

        public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

        public bool Running { get; set; }
    }
}
