using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Services;
using MlHostApi.Models;
using NSwag.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly ILogger<QuestionController> _logger;
        private readonly IQuestion _question;
        private readonly IExecutionContext _executionContext;
        private readonly IOption _option;
        private readonly IJson _json;

        public QuestionController(ILogger<QuestionController> logger, IQuestion question, IExecutionContext executionContext, IOption option, IJson json)
        {
            _logger = logger;
            _question = question;
            _executionContext = executionContext;
            _option = option;
            _json = json;
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
        [OpenApiOperation("Predict", "Executes ML Model and returns intents")]
        [HttpPost()]
        public async Task<ActionResult<PredictResponse>> Predict([FromBody] PredictRequest request)
        {
            _logger.LogInformation($"Predict: {_json.Serialize(request)}");

            if (string.IsNullOrWhiteSpace(request.Sentence)) return StatusCode((int)HttpStatusCode.BadRequest);

            switch (_executionContext.State)
            {
                case ExecutionState.Booting:
                case ExecutionState.Starting:
                    return ReturnNotAvailable();

                case ExecutionState.Running:
                    try
                    {
                        PredictResponse value = await _question.Ask(new Question { Sentence = request.Sentence });
                        _logger.LogInformation($"Predict answer: {_json.Serialize(value)}");
                        return Ok(value);
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
                Status = _executionContext.State.ToString(),
            };

            return StatusCode((int)HttpStatusCode.ServiceUnavailable, response);
        }
    }
}