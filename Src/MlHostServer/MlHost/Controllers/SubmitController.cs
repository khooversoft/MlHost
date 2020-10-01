using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Models;
using MlHost.Services;
using MlHostSdk.Models;
using NSwag.Annotations;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHost.Controllers
{
    [Route("api/[controller]")]
    [Route("api/question")]
    [ApiController]
    public class SubmitController : ControllerBase
    {
        private readonly ILogger<SubmitController> _logger;
        private readonly IPredictService _question;
        private readonly IExecutionContext _executionContext;
        private readonly IJson _json;
        private readonly IOption _option;

        public SubmitController(ILogger<SubmitController> logger, IPredictService question, IExecutionContext executionContext, IJson json, IOption option)
        {
            _logger = logger;
            _question = question;
            _executionContext = executionContext;
            _json = json;
            _option = option;
        }

        /// <summary>
        /// Predict from the question
        /// 
        /// Body: { "sentence":"{data}" }
        /// 
        /// Example: { "sentence" : "I need a flu shot. Where can I get one?" }
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns>result from ML Model</returns>
        [OpenApiOperation("Submit", "Executes ML Model and returns intents")]
        [HttpPost()]
        public async Task<ActionResult<PredictResponse>> Submit([FromBody] PredictRequest request)
        {
            _logger.LogInformation($"{nameof(Submit)}: {_json.Serialize(request)}");

            if (!request.IsValidRequest()) return StatusCode((int)HttpStatusCode.BadRequest);

            switch (_executionContext.State)
            {
                case ExecutionState.Booting:
                case ExecutionState.Starting:
                case ExecutionState.Restarting:
                    return ReturnNotAvailable();

                case ExecutionState.Running:
                    try
                    {
                        PredictResponse hostResponse = await _question.Submit(new Question { Sentence = request.Request ?? request.Sentence });
                        _logger.LogInformation($"{nameof(Submit)} answer: {_json.Serialize(hostResponse)}");

                        var result = new PredictResponse
                        {
                            Model = hostResponse.Model,
                            Request = hostResponse.Request,
                            Intents = hostResponse.Intents
                                .OrderByDescending(x => x.Score)
                                .Take(request.IntentLimit ?? int.MaxValue)
                                .ToList(),
                        };

                        return Ok(result);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Exception from model.  Ex={ex}");
                        throw;
                    }

                default:
                    _logger.LogError($"Failed: ExecutionState={_executionContext.State}");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        private ObjectResult ReturnNotAvailable()
        {
            _logger.LogInformation($"Predict is starting up");

            var response = new PingResponse
            {
                Version = _option.HostVersion(),
                Status = _executionContext.State.ToString(),
            };

            return StatusCode((int)HttpStatusCode.ServiceUnavailable, response);
        }
    }
}