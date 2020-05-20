using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHost.Test.Application;
using MlHostApi.Models;
using System.Collections.Generic;
using System.Net.Http;
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
            TestWebsiteHost host = await TestApplication.GetHost();
            await host.WaitForStartup();

            var question = new PredictRequest
            {
                Sentence = "I am happy",
                MemberKey = "my key",
                Metadata = new Dictionary<string, string>
                {
                    ["Meta1"] = "Meta1-Data1",
                    ["Meta2"] = "Meta1-Data2"
                }
            };

            IJson jsonSerializer = host.Resolve<IJson>();

            string content = jsonSerializer.Serialize(question);
            var response = await host.Client.PostAsync("api/question", new StringContent(content, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().NotBeNullOrEmpty();

            PredictResponse predictResponse = jsonSerializer.Deserialize<PredictResponse>(responseString);
            predictResponse.Should().NotBeNull();

            predictResponse.Query.Should().Be(question.Sentence);

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
