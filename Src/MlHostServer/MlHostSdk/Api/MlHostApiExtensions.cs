using MlHostSdk.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostSdk.Api
{
    public static class MlHostApiExtensions
    {
        [Obsolete("Should use 'Submit' API")]
        public static async Task<PredictResponse> PostMlQuestion(this HttpClient httpClient, PredictRequest predictRequest) =>
            await httpClient.PostAsJsonAsync("api/question", predictRequest)
                .GetContent<PredictResponse>();

        public static async Task<PredictResponse> PostMlRequest(this HttpClient httpClient, PredictRequest predictRequest) =>
            await httpClient.PostAsJsonAsync("api/submit", predictRequest)
                .GetContent<PredictResponse>();

        public static async Task<PingLogs> GetMlLogs(this HttpClient httpClient) => await httpClient.GetFromJsonAsync<PingLogs>("api/ping/Logs");

        public static async Task<BatchResponse> PostMlBatchRequest(this HttpClient httpClient, BatchRequest batchRequest) =>
            await httpClient.PostAsJsonAsync("api/submit", batchRequest)
                .GetContent<BatchResponse>();

        public static async Task<PingResponse> Ping(this HttpClient httpClient) =>
            await httpClient.GetFromJsonAsync<PingResponse>("api/ping");

        public static async Task<PingResponse> PingRunning(this HttpClient httpClient) =>
            await httpClient.GetFromJsonAsync<PingResponse>("api/ping/running");

        public static async Task<PingResponse> PingReady(this HttpClient httpClient) =>
            await httpClient.GetFromJsonAsync<PingResponse>("api/ping/ready");
    }
}
