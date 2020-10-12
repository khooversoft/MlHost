using MlHostSdk.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHostSdk.RestApi
{
    public class RestApiBase<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        protected readonly HttpClient _httpClient;

        public RestApiBase(HttpClient httpClient)
        {
            httpClient.VerifyNotNull(nameof(httpClient));

            _httpClient = httpClient;
            ApiUrl = new ApiUrl();
        }

        public RestApiBase(HttpClient httpClient, string baseAddress)
        {
            httpClient.VerifyNotNull(nameof(httpClient));

            _httpClient = httpClient;
            ApiUrl = new ApiUrl(baseAddress);
        }

        public ApiUrl ApiUrl { get; }

        public async Task<PingLogs> GetLogs() =>
            await _httpClient.GetFromJsonAsync<PingLogs>(ApiUrl.GetLogsUrl());

        public async Task<PingResponse> Ping() =>
            await _httpClient.GetFromJsonAsync<PingResponse>(ApiUrl.GetPingUrl());

        public async Task<PingResponse> PingRunning() =>
            await _httpClient.GetFromJsonAsync<PingResponse>(ApiUrl.GetPingRunningUrl());

        public async Task<PingResponse> PingReady() =>
            await _httpClient.GetFromJsonAsync<PingResponse>(ApiUrl.GetPingReadyUrl());

        public async Task<PostResponse<TResponse>> PostRequest(TRequest request)
        {
            string requestUrl = ApiUrl.GetRequestUrl();
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync(requestUrl, request);

            string contentJson = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (contentJson.IndexOf("start", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return new PostResponse<TResponse>(httpResponseMessage.StatusCode, true, contentJson);
                }
                else
                {
                    return new PostResponse<TResponse>(httpResponseMessage.StatusCode, false, contentJson);
                }
            }

            TResponse predictResponse = Json.Default.Deserialize<TResponse>(contentJson);
            return new PostResponse<TResponse>(httpResponseMessage.StatusCode, predictResponse, contentJson);
        }
    }
}
