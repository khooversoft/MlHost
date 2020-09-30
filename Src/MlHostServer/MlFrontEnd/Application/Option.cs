using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Application;

namespace MlFrontEnd.Application
{
    internal class Option : IOption
    {
        public string? SecretId { get; set; }

        public string Environment { get; set; } = "dev";

        public RunEnvironment RunEnvironment { get; set; } = RunEnvironment.Unknown;

        public string[]? ApplicationUrl { get; set; }

        public IList<HostOption> Hosts { get; set; } = null!;
    }
}
