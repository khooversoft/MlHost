using Microsoft.AspNetCore.Mvc;
using MlHost.Application;
using MlHost.Models;
using MlHost.Tools;
using MlHostSdk.Models;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MlHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly IExecutionContext _executionContext;
        private readonly ITelemetryMemory _telemetryMemory;
        private readonly IOption _option;

        public PingController(IExecutionContext executionContext, ITelemetryMemory telemetryMemory, IOption option)
        {
            _executionContext = executionContext;
            _telemetryMemory = telemetryMemory;
            _option = option;
        }

        /// <summary>
        /// Ping to get current state of the ML Service
        /// 
        /// Booting - System is booting
        /// Starting - Service is starting
        /// Running - Service is running
        /// Failed - Service has failed to start
        /// 
        /// </summary>
        /// <returns>status</returns>
        [OpenApiOperation("Ping", "Returns the state of the server")]
        [HttpGet]
        [ProducesResponseType(typeof(PingResponse), (int)HttpStatusCode.OK)]
        public ActionResult<PingResponse> Ping()
        {
            var response = new PingResponse
            {
                Version = _option.HostVersion(),
                Status = _executionContext.State.ToString(),
            };

            return Ok(response);
        }

        /// <summary>
        /// Return 200 if the server is running
        /// </summary>
        /// <returns></returns>
        [OpenApiOperation("Running", "Returns 200 if the server is running")]
        [HttpGet("running")]
        [ProducesResponseType(typeof(PingResponse), (int)HttpStatusCode.OK)]
        public ActionResult Running()
        {
            var response = new PingResponse
            {
                Version = _option.HostVersion(),
                Status = _executionContext.State.ToString(),
            };

            switch (_executionContext.State)
            {
                case ExecutionState.Running:
                case ExecutionState.Booting:
                case ExecutionState.Starting:
                case ExecutionState.Restarting:
                    return Ok(response);

                default:
                    return StatusCode((int)HttpStatusCode.ServiceUnavailable, response);
            }
        }

        /// <summary>
        /// Returns 200 if the server is ready to process requests
        /// </summary>
        /// <returns></returns>
        [OpenApiOperation("Ready", "Returns 200 if the server is ready to process requests")]
        [HttpGet("ready")]
        [ProducesResponseType(typeof(PingResponse), (int)HttpStatusCode.OK)]
        public ActionResult Ready()
        {
            var response = new PingResponse
            {
                Version = _option.HostVersion(),
                Status = _executionContext.State.ToString(),
            };

            switch (_executionContext.State)
            {
                case ExecutionState.Running:
                    return Ok(response);

                default:
                    return StatusCode((int)HttpStatusCode.ServiceUnavailable, response);
            }
        }

        /// <summary>
        /// Get the last 100 logs in reverse order
        /// </summary>
        /// <returns>array of strings</returns>
        [OpenApiOperation("Logs", "Return the last 100 internal operation logs")]
        [OpenApiIgnore]
        [HttpGet("Logs")]
        public ActionResult<PingLogs> GetLogs()
        {
            IReadOnlyList<string> logs = _telemetryMemory.GetLoggedMessages();

            var response = new PingLogs
            {
                Version = _option.HostVersion(),
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