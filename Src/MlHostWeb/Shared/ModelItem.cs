using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Shared
{
    public class ModelItem
    {
        public string Name { get; set; }
        public string ModelUrl { get; set; }
        public string DocId { get; set; }
        public string DetailDocId { get; set; }
    }

    public static class ModelItemExtensions
    {
        public static string GetSwaggerUrl(this ModelItem subject) => new UriBuilder(subject.ModelUrl)
        {
            Path = "Swagger"
        }.ToString();
    }
}
