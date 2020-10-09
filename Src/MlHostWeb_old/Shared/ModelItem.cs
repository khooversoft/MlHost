using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Shared
{
    public class ModelItem
    {
        public ModelItem() { }

        public string Name { get; set; }
        public string VersionId { get; set; }
        public string ModelUrl { get; set; }
        public string SwaggerUrl { get; set; }
        public string DocId { get; set; }
        public string DetailDocId { get; set; }
        public string LogUrl { get; set; }
    }
}
