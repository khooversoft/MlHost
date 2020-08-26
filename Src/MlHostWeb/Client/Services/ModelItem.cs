using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Services
{
    public class ModelItem
    {
        public ModelItem(string name, string versionId, string docId, string modelUri)
        {
            Name = name;
            VersionId = versionId;
            DocId = docId;
            ModelUri = modelUri;
        }

        public string Name { get; }
        public string VersionId { get; }
        public string DocId { get; }
        public string ModelUri { get; }
    }
}
