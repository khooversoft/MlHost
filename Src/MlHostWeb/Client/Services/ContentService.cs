using Markdig;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Services
{
    public class ContentService : IContentService
    {
        private ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();

        public ContentService() { }

        public string GetDocHtml(string id)
        {
            if (_cache.TryGetValue(id, out string html)) return html;

            return AddResource(id);
        }

        private string AddResource(string id)
        {
            string path = $"MlHostWeb.Client.Application.Data.{id}";

            using Stream resource = Assembly.GetAssembly(typeof(ContentService)).GetManifestResourceStream(path);
            if (resource == null) throw new ArgumentException($"Cannot find doc {id}");

            using StreamReader reader = new StreamReader(resource);
            string resourceHtml = reader.ReadToEnd();

            string mdSource = Transform(resourceHtml);
            _cache.TryAdd(id, mdSource);

            return mdSource;
        }

        private string Transform(string mdSource)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            var result = Markdown.ToHtml(mdSource, pipeline);

            return result;
        }
    }
}
