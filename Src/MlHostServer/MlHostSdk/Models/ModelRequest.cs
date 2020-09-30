using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostSdk.Models
{
    public class ModelRequest
    {
        public string? VersionId { get; set; }

        public int? IntentLimit { get; set; }
    }
}
