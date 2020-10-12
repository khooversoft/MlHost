using Microsoft.Extensions.Logging;
using MlFrontEnd.Application;
using MlHostSdk.Models;
using MlHostSdk.RestApi;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlFrontEnd.Services
{
    public class HostProxyService
    {
        private readonly ConcurrentDictionary<string, ModelRestApi> _clients = new ConcurrentDictionary<string, ModelRestApi>(StringComparer.OrdinalIgnoreCase);
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOption _option;
        private readonly ILogger<HostProxyService> _logger;

        public HostProxyService(IHttpClientFactory httpClientFactory, IOption option, ILogger<HostProxyService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _option = option;
            _logger = logger;

            foreach (var item in _option.Hosts)
            {
                _clients.TryAdd(item.ModelName, new ModelRestApi(_httpClientFactory.CreateClient(item.ModelName)))
                    .VerifyAssert(x => x == true, $"Failed to add http client for {item.ModelName}");
            }
        }

        public async Task<PredictResponse?> Submit(string modelName, PredictRequest predictRequest, CancellationToken cancellationToken)
        {
            predictRequest.VerifyNotNull(nameof(predictRequest));

            _clients.TryGetValue(modelName, out ModelRestApi? modelRestApi)
                .VerifyAssert(x => x == true, $"VersionId={modelName} http client is not registered");

            _logger.LogTrace($"{nameof(Submit)}: Calling model {modelName}, Url={modelRestApi!.ApiUrl.GetRequestUrl()}");

            try
            {
                PostResponse<PredictResponse> response = await modelRestApi.PostRequest(predictRequest);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"Format error for failed call to model {modelName}, Url={modelRestApi!.ApiUrl.GetRequestUrl()}");
                    return null;
                }

                return new PredictResponse
                {
                    Model = new Model
                    {
                        Name = response.Value!.Model?.Name,
                        Version = response.Value.Model?.Version,
                    },

                    Request = response.Value.Request,

                    Intents = (response.Value.Intents ?? Array.Empty<Intent>())
                        .OrderByDescending(x => x.Score)
                        .Take(predictRequest.IntentLimit ?? response.Value.Intents?.Count ?? 0)
                        .ToList(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Format error for failed call to model {modelName}, Url={modelRestApi!.ApiUrl.GetRequestUrl()}");
                return null;
            }
        }
    }
}
