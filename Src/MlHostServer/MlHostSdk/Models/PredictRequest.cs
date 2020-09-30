using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostSdk.Models
{
    public class PredictRequest
    {
        public string? Sentence { get; set; }

        public string? Request { get; set; }

        public int? IntentLimit { get; set; }
    }
}
