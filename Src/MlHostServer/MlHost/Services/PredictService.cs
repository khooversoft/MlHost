using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHostSdk.Models;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHost.Services
{
    internal class PredictService : IPredictService, IDisposable
    {
        private readonly IOption _option;
        private readonly ILogger<PredictService> _logger;
        private readonly IJson _json;
        private readonly SemaphoreSlim _limit;
        private HttpClient _httpClient = new HttpClient();

        public PredictService(IOption option, ILogger<PredictService> logger, IJson json)
        {
            _option = option;
            _logger = logger;
            _json = json;
            _limit = new SemaphoreSlim(_option.MaxRequests);
        }

        public async Task<PredictResponse> Submit(Question request)
        {
            _httpClient.VerifyNotNull($"{nameof(PredictService)} has been disposed");

            await _limit.WaitAsync(TimeSpan.FromMinutes(5));

            try
            {
                _logger.LogTrace($"Sending question '{_json.Serialize(request)}' to model.");

                var sw = Stopwatch.StartNew();
                PredictResponse predictResponse = await _httpClient.PostAsJsonAsync(_option.ServiceUri, request)
                    .GetContent<PredictResponse>();

                _logger.LogInformation($"Receive answer '{_json.Serialize(predictResponse)}' from model for question '{request.Sentence}', ms={sw.ElapsedMilliseconds}");
                return predictResponse;
            }
            finally
            {
                _limit.Release();
            }
        }

        public void Dispose() => Interlocked.Exchange(ref _httpClient, null!)?.Dispose();
    }
}
