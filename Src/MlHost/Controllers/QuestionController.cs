using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Services;
using MlHostApi.Models;
using System.Net;
using System.Threading.Tasks;
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

        public QuestionController(ILogger<QuestionController> logger, IQuestion question, IExecutionContext executionContext, IOption option)
        {
            _logger = logger;
            _question = question;
            _executionContext = executionContext;
            _option = option;
        }

        [HttpPost("predict")]
        public async Task<IActionResult> Predict([FromBody] dynamic request)
        {
            bool requestIsValid = ((object)request).ToDictionary()
                ?.Func(x => x.Count > 0) ?? false;

            _logger.LogInformation($"Predict: {request}");

            if (!requestIsValid) return StatusCode((int)HttpStatusCode.BadRequest);

            switch(_executionContext.State)
            {
                case ExecutionState.Booting:
                case ExecutionState.Starting:
                case ExecutionState.Deploying:
                    return ReturnNotAvailable();

                case ExecutionState.Running:
                    dynamic value = await _question.Ask(request);
                    _logger.LogInformation($"Predict answer: {value}");
                    return Ok(value);

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
                ModelId = _executionContext?.ModelId?.ToString(),
                HostName = _option.HostName,
            };

            return StatusCode((int)HttpStatusCode.ServiceUnavailable, response);
        }
    }
}