using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostSdk.Models
{
    public class BatchResponse
    {
        public string? Request { get; set; }

        public IList<PredictResponse?>? Responses { get; set; }
    }
}
