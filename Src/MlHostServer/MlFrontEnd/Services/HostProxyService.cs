using Microsoft.Extensions.Logging;
using MlFrontEnd.Application;
using MlHostSdk.Api;
using MlHostSdk.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlFrontEnd.Services
{
    public class HostProxyService
    {
        private readonly ConcurrentDictionary<string, HttpClient> _clients = new ConcurrentDictionary<string, HttpClient>(StringComparer.OrdinalIgnoreCase);
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
                _clients.TryAdd(item.VersionId, _httpClientFactory.CreateClient(item.VersionId))
                    .VerifyAssert(x => x == true, $"Failed to add http client for {item.VersionId}");
            }
        }

        public async Task<PredictResponse?> Submit(string versionId, PredictRequest predictRequest, CancellationToken cancellationToken)
        {
            predictRequest.VerifyNotNull(nameof(predictRequest));

            _clients.TryGetValue(versionId, out HttpClient? httpClient)
                .VerifyAssert(x => x == true, $"VersionId={versionId} http client is not registered");

            _logger.LogTrace($"{nameof(Submit)}: Calling model {versionId}, Url={httpClient!.BaseAddress}");

            try
            {
                PredictResponse response = await httpClient.PostMlRequest(predictRequest);

                return new PredictResponse
                {
                    Model = new Model
                    {
                        Name = response.Model?.Name,
                        Version = response.Model?.Version,
                    },

                    Request = response.Request,

                    Intents = (response.Intents ?? Array.Empty<Intent>())
                        .OrderByDescending(x => x.Score)
                        .Take(predictRequest.IntentLimit ?? response.Intents?.Count ?? 0)
                        .ToList(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Format error for failed call to model {versionId}, Url={httpClient!.BaseAddress}");
                return null;
            }
        }
    }
}
