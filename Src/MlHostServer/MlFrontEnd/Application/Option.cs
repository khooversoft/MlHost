using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Application;

namespace MlFrontEnd.Application
{
    internal class Option : IOption
    {
        public string Environment { get; set; } = "dev";
        public IList<HostOption> Hosts { get; set; } = null!;
        public int? Port { get; set; }
        public RunEnvironment RunEnvironment { get; set; } = RunEnvironment.Unknown;
    }
}
