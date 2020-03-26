using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MlHost.Services;
using MlHostApi.Models;
using MlHostApi.Tools;
using System.Diagnostics;
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

        public QuestionController(ILogger<QuestionController> logger, IQuestion question, IExecutionContext executionContext)
        {
            _logger = logger;
            _question = question;
            _executionContext = executionContext;
        }

        [HttpPost("predict")]
        public async Task<IActionResult> Predict([FromBody] QuestionRequest questionRequest)
        {
            if (questionRequest == null || questionRequest.Question.ToNullIfEmpty() == null) return StatusCode((int)HttpStatusCode.BadRequest);

            if (_executionContext.State != ExecutionState.Running)
            {
                _logger.LogInformation($"Predict is starting up");
                return StatusCode((int)HttpStatusCode.NoContent);
            }

            var sw = Stopwatch.StartNew();

            _logger.LogInformation($"Question: {questionRequest.Question}");
            AnswerResponse value = await _question.Ask(questionRequest);

            sw.Stop();
            _logger.LogInformation($"Question: {questionRequest.Question}, completed: {sw.ElapsedMilliseconds}ms");

            return Ok(value);
        }
    }
}