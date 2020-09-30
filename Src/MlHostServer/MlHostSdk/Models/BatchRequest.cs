using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MlHostSdk.Models
{
    public class BatchRequest
    {
        public string Request { get; set; } = null!;

        public IList<ModelRequest> Models { get; set; } = null!;
    }
}
