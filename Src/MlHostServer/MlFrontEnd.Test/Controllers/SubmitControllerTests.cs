using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlFrontEnd.Test.Application;
using MlHostSdk.Models;
using MlHostSdk.RestApi;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlFrontEnd.Test.Controllers
{
    [TestClass]
    public class SubmitControllerTests
    {
        [TestMethod]
        public async Task GivenBatchRequest_WhenSentToOneHost_ReceiveTwoResults()
        {
            TestFrontendHost host = TestApplication.StartHosts();

            var batchRequest = new BatchRequest
            {
                Request = "I am happy",
                Models = TestApplication.ModelNames.Select(x => new ModelRequest
                {
                    ModelName = x,
                    IntentLimit = 1,
                }).ToList(),
            };

            PostResponse<BatchResponse> response = await new FrontEndRestApi(host.HttpClient).PostRequest(batchRequest);
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            BatchResponse batchResponse = response.Value!;
            batchResponse.Should().NotBeNull();

            batchResponse.Request.Should().Be(batchRequest.Request);
            batchResponse.Responses.Should().NotBeNull();
            batchResponse.Responses!.Count.Should().Be(TestApplication.ModelNames.Count);
            batchResponse.Responses.All(x => x?.Model?.Name.ToNullIfEmpty() != null).Should().BeTrue();

            TestApplication.ModelNames.OrderBy(x => x)
                .Zip(batchResponse.Responses.OrderBy(x => x!.Model!.Name), (versionId, batchResponse) => (versionId, batchResponse))
                .ForEach(x =>
                {
                    x.batchResponse.Should().NotBeNull();
                    x.batchResponse!.Model.Should().NotBeNull();
                    x.batchResponse.Model!.Name.Should().Be(x.versionId);
                    x.batchResponse.Model.Version.Should().Be("1.0");
                    x.batchResponse.Request.Should().Be(batchRequest.Request);
                    x.batchResponse.Intents.Should().NotBeNull();
                    x.batchResponse.Intents.Count().Should().Be(1);
                    x.batchResponse.Intents.First().Label.Should().Be("HAPPINESS");
                    x.batchResponse.Intents.First().Score.Should().Be(0.9824827);
                });
        }
    }
}
