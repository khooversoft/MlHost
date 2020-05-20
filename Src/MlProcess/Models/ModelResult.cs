using MlHostApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlProcess.Models
{
    internal class ModelResult
    {
        public string? ModelName { get; set; }

        public IList<Intent>? Intents { get; set; }
    }
}
