using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MlHost.Application;
using MlHost.Services;
using MlHost.Tools;
using MlHostApi.Models;
using MlHostApi.Tools;
using System.Net;
using System.Threading.Tasks;

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

            if (!requestIsValid) return StatusCode((int)HttpStatusCode.BadRequest);

            if (_executionContext.State != ExecutionState.Running) return ReturnNotAvailable();

            dynamic value = await _question.Ask(request);
            return Ok(value);
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