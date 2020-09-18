using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MlHostApi.Models
{
    public class PredictResponse
    {
        public Model? Model { get; set; }

        public string? Query { get; set; }

        public IList<Intent>? Intents { get; set; }

        public IList<Intent>? Intent { get; set; }

        public IList<Intent> GetTopIntents() => (Intent ?? Intents ?? Array.Empty<Intent>())
            .OrderByDescending(x => x.Score)
            .Take(5)
            .ToList();

        public IList<Intent> GetIntents() => (Intent ?? Intents ?? Array.Empty<Intent>())
            .OrderByDescending(x => x.Score)
            .ToList();
    }
}
