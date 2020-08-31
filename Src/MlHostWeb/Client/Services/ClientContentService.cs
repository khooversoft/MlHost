using MlHostWeb.Shared;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Services
{
    public class ClientContentService
    {
        private readonly HttpClient _httpClient;
        private ConcurrentDictionary<string, DocItem> _cache = new ConcurrentDictionary<string, DocItem>();

        public ClientContentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetDocHtml(string id)
        {
            DocItem docItem;

            if (_cache.TryGetValue(id, out docItem)) return docItem.Html;

            docItem = await _httpClient.GetFromJsonAsync<DocItem>($"api/config/doc/{id}");
            _cache.TryAdd(id, docItem);

            return docItem.Html;
        }
    }
}
