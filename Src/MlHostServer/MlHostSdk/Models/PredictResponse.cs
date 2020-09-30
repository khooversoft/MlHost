using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MlHostSdk.Models
{
    public class PredictResponse
    {
        public Model? Model { get; set; }

        public string? Request { get; set; }

        public IList<Intent>? Intents { get; set; }
    }
}
