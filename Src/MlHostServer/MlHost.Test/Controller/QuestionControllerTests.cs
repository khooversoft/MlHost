using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHost.Test.Application;
using MlHostSdk.Models;
using MlHostSdk.RestApi;
using System.Linq;
using System.Threading.Tasks;

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

            var request = new PredictRequest
            {
                Request = "I am sad",
            };

            PredictResponse predictResponse = (await new ModelRestApi(host.Client).PostRequest(request)).Value!;
            Verify(predictResponse, request.Request);
        }

        [TestMethod]
        public async Task GivenTestModelOldApi_WhenUsed_ShouldResponed()
        {
            TestWebsiteHost host = await TestApplication.GetHost();
            await host.WaitForStartup();

            var question = new PredictRequest
            {
                Sentence = "I am happy",
            };

#pragma warning disable CS0618 // Type or member is obsolete
            PredictResponse predictResponse = await host.Client.PostMlQuestion(question);
#pragma warning restore CS0618 // Type or member is obsolete

            Verify(predictResponse, question.Sentence);
        }


        private void Verify(PredictResponse predictResponse, string request)
        {
            predictResponse.Should().NotBeNull();

            predictResponse.Request.Should().Be(request);

            predictResponse.Model.Should().NotBeNull();
            predictResponse.Model!.Name.Should().Be("fakeModel");
            predictResponse.Model!.Version.Should().Be("1.0");

            predictResponse.Intents.Should().NotBeNull();
            predictResponse.Intents!.Count.Should().BeGreaterThan(2);
            predictResponse.Intents.First().Label.Should().Be("HAPPINESS");
            predictResponse.Intents.First().Score.Should().Be(0.9824827);
            predictResponse.Intents.Last().Label.Should().Be("LOVE");
            predictResponse.Intents.Last().Score.Should().Be(0.009116333);
        }
    }
}
