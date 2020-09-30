using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MlFrontEnd.Application;
using MlFrontEnd.Services;
using MlHostSdk.Models;
using NSwag.Annotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;

namespace MlFrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmitController : ControllerBase
    {
        private readonly HostProxyService _hostProxyService;
        private readonly BatchService _batchService;
        private readonly IJson _json;
        private readonly ILogger<SubmitController> _logger;

        public SubmitController(HostProxyService hostProxyService, BatchService batchService, IJson json, ILogger<SubmitController> logger)
        {
            _hostProxyService = hostProxyService;
            _batchService = batchService;
            _json = json;
            _logger = logger;
        }

        /// <summary>
        /// Process batch request
        /// </summary>
        /// <param name="request"></param>
        /// <returns>result from ML Model</returns>
        [OpenApiOperation("Submit", "Executes batch requests")]
        [HttpPost()]
        [ProducesResponseType(typeof(BatchResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BatchResponse>> Process([FromBody] BatchRequest request, CancellationToken token)
        {
            _logger.LogInformation($"{nameof(Question)}: {_json.Serialize(request)}");

            if (request.IsValidRequest()) return StatusCode((int)HttpStatusCode.BadRequest);

            return await _batchService.Submit(request, token);
        }

        /// <summary>
        /// Process model request
        /// 
        /// Body: { "request" : "{data}" }
        /// 
        /// Example: { "request" : "I need a flu shot. Where can I get one?" }
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns>result from ML Model</returns>
        [OpenApiOperation("Submit", "Executes specific ML Model and returns intents")]
        [HttpPost("{versionId}")]
        [ProducesResponseType(typeof(PredictResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PredictResponse>> ProcessVersionId(string versionId, [FromBody] PredictRequest request, CancellationToken token)
        {
            _logger.LogInformation($"{nameof(Question)}: {_json.Serialize(request)}");

            if (request.IsValidRequest()) return StatusCode((int)HttpStatusCode.BadRequest);

            return Ok(await _hostProxyService.Submit(versionId, request, token));
        }
    }
}
