using Microsoft.Extensions.Logging;
using MlHost.Application;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;

namespace MlHost.Services
{
    internal class QuestionService : IQuestion, IDisposable
    {
        private readonly IOption _option;
        private readonly ILogger<QuestionService> _logger;
        private readonly IJson _json;
        private HttpClient _httpClient = new HttpClient();

        public QuestionService(IOption option, ILogger<QuestionService> logger, IJson json)
        {
            _option = option;
            _logger = logger;
            _json = json;
        }

        public async Task<dynamic> Ask(dynamic request)
        {
            string json = _json.Serialize(request);

            _logger.LogTrace($"Sending question '{json}' to model.");

            var sw = Stopwatch.StartNew();
            HttpResponseMessage response = await _httpClient.PostAsync(_option.ServiceUri, new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            sw.Stop();

            string responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Receive answer '{json}' from model for question '{responseContent}', ms={sw.ElapsedMilliseconds}");

            return _json.Deserialize<dynamic>(responseContent);
        }

        public void Dispose() => Interlocked.Exchange(ref _httpClient, null!)?.Dispose();
    }
}
