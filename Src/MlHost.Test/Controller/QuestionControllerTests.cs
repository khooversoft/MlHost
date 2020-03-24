using FluentAssertions;
using MlHost.Services;
using MlHost.Test.Application;
using MlHostApi.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MlHost.Test.Controller
{
    [Collection("WebsiteTest")]
    public class QuestionControllerTests : IClassFixture<StorageStoreFixture>
    {
        private readonly StorageStoreFixture _storageStoreFixture;

        public QuestionControllerTests(StorageStoreFixture storageStoreFixture)
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
            answerModel.Start.Should().NotBe(0);
            answerModel.End.Should().NotBe(0);
            answerModel.Answer.Should().NotBeNullOrEmpty();
            answerModel.Score.Should().NotBe(0d);
        }
    }
}
