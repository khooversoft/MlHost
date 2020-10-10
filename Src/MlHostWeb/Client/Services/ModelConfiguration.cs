using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MlHostSdk.RestApi;
using MlHostWeb.Client.Application;
using MlHostWeb.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHostWeb.Client.Services
{
    public class ModelConfiguration
    {
        private Configuration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ConcurrentDictionary<string, ModelRestApi> _apiCache = new ConcurrentDictionary<string, ModelRestApi>(StringComparer.OrdinalIgnoreCase);

        public ModelConfiguration(HttpClient httpClient) => _httpClient = httpClient;

        public async Task Initialize() => _configuration ??= (await _httpClient.GetFromJsonAsync<Configuration>("api/config"));

        public ModelItem GetModel(string modelName)
        {
            var modelItem = _configuration
                .Models
                .FirstOrDefault(x => x.Name == modelName);

            return modelItem;
        }

        public IReadOnlyList<ModelItem> GetModels() => _configuration.Models
            .OrderBy(x => x.Name)
            .ToList();

        public ModelRestApi GetModelApi(string modelName)
        {
            return _apiCache.GetOrAdd(modelName, getRestApi);

            ModelRestApi getRestApi(string key) => new ModelRestApi(_httpClient, getModelItem(key).ModelUrl);

            ModelItem getModelItem(string modelName) => GetModel(modelName)
                .VerifyNotNull($"{nameof(GetModelApi)}: Cannot find {modelName}");
        }
    }
}
