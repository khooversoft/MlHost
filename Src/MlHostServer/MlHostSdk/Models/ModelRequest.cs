using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostSdk.Models
{
    public class ModelRequest
    {
        public string? ModelName { get; set; }

        public int? IntentLimit { get; set; }
    }
}
