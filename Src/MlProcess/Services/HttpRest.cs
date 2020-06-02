using Microsoft.Extensions.Logging;
using MlHostApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlProcess.Services
{
    internal class HttpRest
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly TimeSpan? _waitTime = TimeSpan.FromMinutes(1);
        private readonly IJson _json;
        private readonly ILogger<HttpRest> _logger;
        private readonly string _uri;
        private DateTime? _breaker;

        public HttpRest(IJson json, ILogger<HttpRest> logger, string uri)
        {
            _json = json;
            _logger = logger;
            _uri = uri;
        }

        public async Task<PredictResponse> Invoke(string question, CancellationToken token)
        {
            question.VerifyNotEmpty(nameof(question));

            var request = new PredictRequest
            {
                Sentence = question,
            };

            if (_breaker != null && _breaker + _waitTime > DateTime.Now)
            {
                string msg = $"Breaker is set for {_uri}";
                _logger.LogTrace(msg);
                throw new CircuitBreakerException(msg);
            }

            _breaker = null;

            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(_uri))
                {
                    Content = new StringContent(_json.Serialize(request), Encoding.UTF8, "application/json"),
                };

                HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest, token);
                httpResponse.EnsureSuccessStatusCode();

                string responseJson = await httpResponse.Content.ReadAsStringAsync();
                return _json.Deserialize<PredictResponse>(responseJson);
            }
            catch (TaskCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Rest API failed {_uri}");
                _breaker = DateTime.Now;
                throw;
            }
        }
    }
}
