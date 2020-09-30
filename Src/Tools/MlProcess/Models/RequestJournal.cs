using MlHostSdk.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlProcess.Models
{
    internal class RequestJournal
    {
        public int RequestId { get; set; }

        public string? Question { get; set; }
    }
}
