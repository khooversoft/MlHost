using FluentAssertions;
using MlHost.Services;
using MlHost.Test.Application;
using MlHostApi.Models;
using MlHostApi.Services;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MlHost.Test.Controller
{
    [Collection("WebsiteTest")]
    public class QuestionControllerTests : IClassFixture<TestHostWithStorage>
    {
        private readonly TestHostWithStorage _storageStoreFixture;

        public QuestionControllerTests(TestHostWithStorage storageStoreFixture)
        {
            _storageStoreFixture = storageStoreFixture;
        }

        [Fact]
        public async Task GivenTestModel_WhenUsed_ShouldResponed()
        {
            await _storageStoreFixture.WaitForStartup();

            var question = new QuestionRequest
            {
                Question = "what is my deductible",
            };

            IJson jsonSerializer = _storageStoreFixture.Resolve<IJson>();

            string content = jsonSerializer.Serialize(question);
            var response = await _storageStoreFixture.Client.PostAsync("api/question/predict", new StringContent(content, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            AnswerResponse answerModel = jsonSerializer.Deserialize<AnswerResponse>(responseString);

            answerModel.Should().NotBeNull();
            answerModel.Answer.Should().Be("{label=2; score=0.999996602535248}");
        }
    }
}
