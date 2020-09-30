using Microsoft.Extensions.Logging;
using MlFrontEnd.Application;
using MlHostSdk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlFrontEnd.Services
{
    public class BatchService
    {
        private readonly HostProxyService _hostProxyService;
        private readonly IOption _option;
        private readonly ILogger<BatchService> _logger;

        public BatchService(HostProxyService hostProxyService, IOption option, ILogger<BatchService> logger)
        {
            _hostProxyService = hostProxyService;
            _option = option;
            _logger = logger;
        }

        public async Task<BatchResponse> Submit(BatchRequest batchRequest, CancellationToken token)
        {
            batchRequest.VerifyNotNull(nameof(batchRequest));

            _logger.LogTrace($"{nameof(Submit)}: Executing batch");

            ExecuteRequest[] executeRequests = _option.Hosts
                .Join(batchRequest.Models, x => x.VersionId, x => x.VersionId, (host, request) => new ExecuteRequest(request, host), StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return new BatchResponse
            {
                Request = batchRequest.Request,
                Responses = (await Submit(executeRequests, batchRequest.Request, token)).ToList(),
            };
        }

        public async Task<IReadOnlyList<PredictResponse?>> Submit(ExecuteRequest[] executeRequests, string request, CancellationToken token)
        {
            executeRequests.VerifyNotNull(nameof(executeRequests));
            executeRequests.VerifyAssert(x => x.Length > 0, "Host option is required");
            request.VerifyNotNull(nameof(request));

            _logger.LogTrace($"{nameof(Submit)}: Calling {executeRequests.Length} models");

            Task<PredictResponse?>[] tasks = executeRequests
                .Select(x => _hostProxyService.Submit(x.ModelRequest.VersionId!, new PredictRequest
                {
                    Request = request,
                    IntentLimit = x.ModelRequest.IntentLimit
                }, token))
                .ToArray();

            PredictResponse?[] responses = await Task.WhenAll(tasks);
            return responses;
        }
    }
}
