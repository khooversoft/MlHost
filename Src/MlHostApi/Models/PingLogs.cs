using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace MlHostApi.Models
{
    public class PingLogs
    {
        public int Count { get; set; }

        public IList<string>? Messages { get; set; }
    }
}
