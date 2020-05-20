using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostApi.Models
{
    public class PredictResponse
    {
        public Model? Model { get; set; }

        public string? Query { get; set; }

        public IList<Intent>? Intents { get; set; }
    }
}
