using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHost.Test.Application;
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

            dynamic question = new
            {
                sentence = "what is my deductible",
            };

            IJson jsonSerializer = host.Resolve<IJson>();

            string content = jsonSerializer.Serialize(question);
            var response = await host.Client.PostAsync("api/question", new StringContent(content, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            responseString.Should().NotBeNullOrEmpty();
            responseString.Should().Be("{\"label\":\"2\",\"score\":\"0.999996602535248\"}");

            var logResponse = await host.Client.GetAsync("api/ping/Logs");
            logResponse.EnsureSuccessStatusCode();

            string logResponseString = await logResponse.Content.ReadAsStringAsync();
            logResponseString.Should().NotBeNullOrEmpty();
        }
    }
}
