using Microsoft.AspNetCore.Mvc;
using MlHost.Application;
using MlHost.Services;
using MlHost.Tools;
using MlHostApi.Models;
using System.Linq;

namespace MlHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly IExecutionContext _executionContext;
        private readonly ITelemetryMemory _telemetryMemory;

        public PingController(IExecutionContext executionContext, ITelemetryMemory telemetryMemory)
        {
            _executionContext = executionContext;
            _telemetryMemory = telemetryMemory;
        }

        [HttpGet]
        public IActionResult Ping()
        {
            var response = new PingResponse
            {
                Status = _executionContext.State.ToString(),
            };

            return Ok(response);
        }

        [HttpGet("Logs")]
        public IActionResult GetLogs()
        {
            var logs = _telemetryMemory.GetLoggedMessages();

            dynamic response = new
            {
                Count = logs.Count,

                Messages = logs
                    .Reverse()
                    .Take(100)
                    .ToList(),
            };

            return Ok(response);
        }
    }
}