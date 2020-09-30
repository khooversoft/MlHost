using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHost.Test.Application;
using MlHostSdk.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Services;

namespace MlHost.Test.Controller
{
    [TestClass]
    public class QuestionControllerTests
    {
        [TestMethod]
        public async Task GivenTestModel_WhenUsed_ShouldResponed()
        {
            var request = new PredictRequest
            {
                Request = "I am happy",
            };

            await ExecutePredict(request);
        }

        [TestMethod]
        public async Task GivenTestModelOldApi_WhenUsed_ShouldResponed()
        {
            var question = new PredictRequest
            {
                Sentence = "I am happy",
            };

            await ExecutePredict(question);
        }

        private async Task ExecutePredict(PredictRequest predictRequest)
        {
            TestWebsiteHost host = await TestApplication.GetHost();
            await host.WaitForStartup();

            IJson jsonSerializer = host.Resolve<IJson>();

            HttpResponseMessage response = await host.Client.PostAsJsonAsync("api/question", predictRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().NotBeNullOrEmpty();

            PredictResponse predictResponse = jsonSerializer.Deserialize<PredictResponse>(responseString);
            predictResponse.Should().NotBeNull();

            predictResponse.Request.Should().Be(predictRequest.Sentence);

            predictResponse.Model.Should().NotBeNull();
            predictResponse.Model!.Name.Should().Be("emote-sent");
            predictResponse.Model!.Version.Should().Be("0.1");

            predictResponse.Intents.Should().NotBeNull();
            predictResponse.Intents!.Count.Should().Be(2);
            predictResponse.Intents![0].Label.Should().Be("HAPPINESS");
            predictResponse.Intents![0].Score.Should().Be(0.9824827);
            predictResponse.Intents![1].Label.Should().Be("LOVE");
            predictResponse.Intents![1].Score.Should().Be(0.009116333);

            var logResponse = await host.Client.GetAsync("api/ping/Logs");
            logResponse.EnsureSuccessStatusCode();

            string logResponseString = await logResponse.Content.ReadAsStringAsync();
            logResponseString.Should().NotBeNullOrEmpty();

        }
    }
}
