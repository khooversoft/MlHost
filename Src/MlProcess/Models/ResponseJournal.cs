using MlHostApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlProcess.Models
{
    internal class ResponseJournal
    {
        public int RequestId { get; set; }

        public string? Question { get; set; }

        public string? ModelName { get; set; }

        public IList<Intent>? Intents { get; set; }
    }
}
