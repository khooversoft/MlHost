using FluentAssertions;
using MlHost.Models;
using MlHost.Test.Application;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace MlHost.Test.Controller
{
    public class QuestionControllerTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public QuestionControllerTests(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task WhenUsingRegistrationApi_WhenPingRequested_ReturnedOk()
        {
            await _fixture.WaitForStartup();

            var question = new QuestionModel
            {
                Question = "what is my deductible",
            };

            string content = _fixture.Json.Serialize(question);
            var response = await _fixture.Client.PostAsync("api/question/predict", new StringContent(content, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            AnswerModel answerModel = _fixture.Json.Deserialize<AnswerModel>(responseString);

            answerModel.Should().NotBeNull();
            answerModel.Start.Should().NotBe(0);
            answerModel.End.Should().NotBe(0);
            answerModel.Answer.Should().NotBeNullOrEmpty();
            answerModel.Score.Should().NotBe(0d);
        }
    }
}
