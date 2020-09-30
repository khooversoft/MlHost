using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlFrontEnd.Application
{
    public class HostOption
    {
        public string VersionId { get; set; } = null!;

        public string Uri { get; set; } = null!;

        public string? RunningUri { get; set; }

        public string? ReadyUri { get; set; }
    }
}
