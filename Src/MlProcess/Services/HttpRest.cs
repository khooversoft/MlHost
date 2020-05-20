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
    internal class HttpRest : IHttpRest
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IJson _json;
        private readonly ILogger<HttpRest> _logger;
        private readonly Random _random = new Random();

        public HttpRest(IJson json, ILogger<HttpRest> logger)
        {
            _json = json;
            _logger = logger;
        }

        public async Task<PredictResponse> Invoke(string uri, string question, CancellationToken token)
        {
            uri.VerifyNotEmpty(nameof(uri));
            question.VerifyNotEmpty(nameof(question));

            var request = new PredictRequest
            {
                Sentence = question,
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(uri))
            {
                Content = new StringContent(_json.Serialize(request), Encoding.UTF8, "application/json"),
            };

            HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest, token);
            httpResponse.EnsureSuccessStatusCode();

            string responseJson = await httpResponse.Content.ReadAsStringAsync();
            return _json.Deserialize<PredictResponse>(responseJson);
        }
    }
}
