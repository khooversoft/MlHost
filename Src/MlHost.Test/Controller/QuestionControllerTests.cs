//using FluentAssertions;
//using MlHost.Models;
//using MlHost.Services;
//using MlHost.Test.Application;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//using Xunit;

//namespace MlHost.Test.Controller
//{
//    [Collection("WebsiteTest")]
//    public class QuestionControllerTests : IClassFixture<LocalStoreFixture>
//    {
//        private readonly LocalStoreFixture _localStoreFixture;

//        public QuestionControllerTests(LocalStoreFixture localStoreFixture)
//        {
//            _localStoreFixture = localStoreFixture;
//        }

//        [Fact]
//        public async Task WhenUsingRegistrationApi_WhenPingRequested_ReturnedOk()
//        {
//            await _localStoreFixture.WaitForStartup();

//            var question = new QuestionModel
//            {
//                Question = "what is my deductible",
//            };

//            IJson jsonSerializer = _localStoreFixture.Resolve<IJson>();

//            string content = jsonSerializer.Serialize(question);
//            var response = await _localStoreFixture.Client.PostAsync("api/question/predict", new StringContent(content, Encoding.UTF8, "application/json"));
//            response.EnsureSuccessStatusCode();

//            var responseString = await response.Content.ReadAsStringAsync();
//            AnswerModel answerModel = jsonSerializer.Deserialize<AnswerModel>(responseString);

//            answerModel.Should().NotBeNull();
//            answerModel.Start.Should().NotBe(0);
//            answerModel.End.Should().NotBe(0);
//            answerModel.Answer.Should().NotBeNullOrEmpty();
//            answerModel.Score.Should().NotBe(0d);
//        }
//    }
//}
