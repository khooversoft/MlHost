using MlHost.Application;
using MlHostApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Services
{
    internal class ExecutionContext : IExecutionContext
    {
        public ExecutionContext() { }

        public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

        public ExecutionState State { get; set; }

        public ModelId? ModelId { get; set; }

        public string? LastException { get; set; }

        public bool ForceDeployment { get; set; }
    }
}
