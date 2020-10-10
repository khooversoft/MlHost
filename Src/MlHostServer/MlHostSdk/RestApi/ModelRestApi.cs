using MlHostSdk.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHostSdk.RestApi
{
    public class ModelRestApi
    {
        private readonly HttpClient _httpClient;

        public ModelRestApi(HttpClient httpClient)
        {
            httpClient.VerifyNotNull(nameof(httpClient));

            _httpClient = httpClient;
            ModelUrl = new ModelUrl();
        }

        public ModelRestApi(HttpClient httpClient, string baseAddress)
        {
            httpClient.VerifyNotNull(nameof(httpClient));

            _httpClient = httpClient;
            ModelUrl = new ModelUrl(baseAddress);
        }

        public ModelUrl ModelUrl { get; }

        public async Task<PredictResponse> PostMlRequest(PredictRequest predictRequest) =>
            await _httpClient.PostAsJsonAsync(ModelUrl.GetRequestUrl(), predictRequest)
                .GetContent<PredictResponse>();

        public async Task<PingLogs> GetMlLogs() =>
            await _httpClient.GetFromJsonAsync<PingLogs>(ModelUrl.GetLogsUrl());

        public async Task<BatchResponse> PostMlBatchRequest(BatchRequest batchRequest) =>
            await _httpClient.PostAsJsonAsync(ModelUrl.GetBatchRequestUrl(), batchRequest)
                .GetContent<BatchResponse>();

        public async Task<PingResponse> Ping() =>
            await _httpClient.GetFromJsonAsync<PingResponse>(ModelUrl.GetPingUrl());

        public async Task<PingResponse> PingRunning() =>
            await _httpClient.GetFromJsonAsync<PingResponse>(ModelUrl.GetPingRunningUrl());

        public async Task<PingResponse> PingReady() =>
            await _httpClient.GetFromJsonAsync<PingResponse>(ModelUrl.GetPingReadyUrl());
    }
}
