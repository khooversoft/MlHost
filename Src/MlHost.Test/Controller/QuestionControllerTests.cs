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
            var host = TestHostWithStorage.GetHost();

            await host.WaitForStartup();

            dynamic question = new
            {
                sentence = "what is my deductible",
            };

            IJson jsonSerializer = host.Resolve<IJson>();

            string content = jsonSerializer.Serialize(question);
            var response = await host.Client.PostAsync("api/question/predict", new StringContent(content, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            responseString.Should().NotBeNullOrEmpty();
            responseString.Should().Be("{\"label\":\"2\",\"score\":\"0.999996602535248\"}");
        }
    }
}
