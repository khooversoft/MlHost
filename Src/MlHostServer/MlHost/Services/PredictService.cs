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

        public async Task<PredictResponse> Submit(Question question)
        {
            _httpClient.VerifyNotNull($"{nameof(PredictService)} has been disposed");

            await _limit.WaitAsync(TimeSpan.FromMinutes(5));

            try
            {
                string requestUri = _option.PropertyResolver.Resolve(_option.ServiceUri);

                _logger.LogInformation($"Sending question '{_json.Serialize(question)}' to model at {requestUri}.");

                var sw = Stopwatch.StartNew();
                PredictResponse predictResponse = await _httpClient.PostAsJsonAsync(requestUri, question)
                    .GetContent<PredictResponse>();

                predictResponse.Request ??= question.Sentence;

                _logger.LogInformation($"Receive answer '{_json.Serialize(predictResponse)}' from model for question '{question.Sentence}', ms={sw.ElapsedMilliseconds}");
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
