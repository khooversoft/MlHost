using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Services
{
    public class ModelService : IModelService
    {
        private static IReadOnlyList<ModelItem> _modelItems = new[]
        {
            new ModelItem("Intent", "intent-v1", "Intent.md", "http://localhost:5010"),
            new ModelItem("Emotion", "emotion-v2", "Emotion.md", "http://localhost:5010"),
            new ModelItem("IdCard", "idcard-v1", "IdCard.md", "http://localhost:5010"),
            new ModelItem("Sentiment", "sentiment-v2", "Sentiment.md", "http://localhost:5010"),
        }
        .OrderBy(x => x.Name)
        .ToArray();

        public ModelService() { }

        public IReadOnlyList<ModelItem> Items => _modelItems;

        public ModelItem GetModel(string versionId)
        {
            return _modelItems.FirstOrDefault(x => x.VersionId == versionId) ?? throw new ArgumentException($"Version id {versionId} is unknown");
        }
    }
}
