using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHostApi.Models;
using MlHostApi.Services;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Services
{
    internal class QuestionService : IQuestion, IDisposable
    {
        private readonly IOption _option;
        private readonly ILogger<QuestionService> _logging;
        private readonly IJson _json;
        private HttpClient _httpClient = new HttpClient();

        public QuestionService(IOption option, ILogger<QuestionService> logging, IJson json)
        {
            _option = option;
            _logging = logging;
            _json = json;
        }

        public async Task<AnswerResponse> Ask(QuestionRequest questionModel)
        {
            questionModel = questionModel ?? throw new AggregateException(nameof(questionModel));
            if (string.IsNullOrWhiteSpace(questionModel.Question)) throw new ArgumentException(nameof(questionModel.Question));

            _logging.LogTrace($"Question: {questionModel.Question}");

            dynamic request = new
            {
                sentence = questionModel.Question,
            };

            var response = await _httpClient.PostAsync(_option.ServiceUri, new StringContent(_json.Serialize(request), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            return new AnswerResponse
            {
                Answer = await response.Content.ReadAsStringAsync(),
            };
        }

        public void Dispose() => Interlocked.Exchange(ref _httpClient, null!)?.Dispose();
    }
}
