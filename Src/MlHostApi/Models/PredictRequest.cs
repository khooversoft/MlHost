using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostApi.Models
{
    public class PredictRequest
    {
        public string? Sentence { get; set; }

        public string? MemberKey { get; set; }

        public IDictionary<string, string>? Metadata { get; set; }
    }
}
