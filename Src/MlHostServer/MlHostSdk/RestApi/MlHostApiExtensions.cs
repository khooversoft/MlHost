using MlHostSdk.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostSdk.RestApi
{
    public static class MlHostApiExtensions
    {
        [Obsolete("Should use 'PostMlRequest' API")]
        public static async Task<PredictResponse> PostMlQuestion(this HttpClient httpClient, PredictRequest predictRequest) =>
            await httpClient.PostAsJsonAsync("api/question", predictRequest)
                .GetContent<PredictResponse>();
    }
}
