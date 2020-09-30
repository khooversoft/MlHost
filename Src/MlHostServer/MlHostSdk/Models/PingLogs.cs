using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace MlHostSdk.Models
{
    public class PingLogs
    {
        public string? Version { get; set; }

        public int Count { get; set; }

        public IList<string>? Messages { get; set; }
    }
}
