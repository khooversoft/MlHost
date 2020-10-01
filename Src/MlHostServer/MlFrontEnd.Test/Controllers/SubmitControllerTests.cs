//using FluentAssertions;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.Extensions.Logging;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using MlFrontEnd.Test.Application;
//using MlHostSdk.Models;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using Toolbox.Services;

//namespace MlFrontEnd.Test.Controllers
//{
//    [TestClass]
//    public class SubmitControllerTests
//    {
//        [TestMethod]
//        public async Task GivenBatchRequest_WhenSentToOneHost_ReceiveTwoResults()
//        {
//            const string versionId = "model1";
//            TestFrontendHost host = await TestApplication.StartHosts(new[] { versionId });

//            var batchRequest = new BatchRequest
//            {
//                Request = "I am happy",
//                Models = new List<ModelRequest>()
//                {
//                    new ModelRequest
//                    {
//                        VersionId = versionId,
//                        IntentLimit = 1,
//                    }
//                }
//            };

//            IJson jsonSerializer = host.Resolve<IJson>();
//            string content = jsonSerializer.Serialize(batchRequest);

//            var response = await host.HttpClient.PostAsync("api/submit", new StringContent(content, Encoding.UTF8, "application/json"));
//            response.EnsureSuccessStatusCode();

//            var responseString = await response.Content.ReadAsStringAsync();
//            responseString.Should().NotBeNullOrEmpty();

//            BatchResponse batchResponse = jsonSerializer.Deserialize<BatchResponse>(responseString);
//            batchResponse.Should().NotBeNull();

//            batchResponse.Request.Should().Be(batchRequest.Request);
//        }
//    }
//}
