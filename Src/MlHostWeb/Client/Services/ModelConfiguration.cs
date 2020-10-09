using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MlHostWeb.Client.Application;
using MlHostWeb.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Services
{
    public class ModelConfiguration
    {
        private Configuration _configuration;
        private readonly HttpClient _httpClient;
        private Guid _instance = Guid.NewGuid();

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
    }
}
