using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHostApi.Models;
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
        private readonly IJson _json;
        private readonly ILogger<QuestionService> _logging;
        private HttpClient _httpClient = new HttpClient();

        public QuestionService(IOption option, IJson json, ILogger<QuestionService> logging)
        {
            _option = option;
            _json = json;
            _logging = logging;
        }

        public async Task<AnswerResponse> Ask(QuestionRequest questionModel)
        {
            questionModel = questionModel ?? throw new AggregateException(nameof(questionModel));
            if (string.IsNullOrWhiteSpace(questionModel.Question)) throw new ArgumentException(nameof(questionModel.Question));

            _logging.LogTrace($"Question: {questionModel.Question}");

            string content = _json.Serialize(questionModel);
            var response = await _httpClient.PostAsync(_option.ServiceUri, new StringContent(content, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            AnswerResponse answerModel = _json.Deserialize<AnswerResponse>(responseString);

            return answerModel;
        }

        public void Dispose() => Interlocked.Exchange(ref _httpClient, null!)?.Dispose();
    }
}
