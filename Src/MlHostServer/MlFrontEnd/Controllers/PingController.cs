using Microsoft.AspNetCore.Mvc;
using MlFrontEnd.Application;
using MlHostSdk.Models;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MlFrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly IOption _option;

        public PingController(IOption option)
        {
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
        public ActionResult<PingResponse> Ping()
        {
            return Ok(GetOkResponse());
        }

        /// <summary>
        /// Return 200 if the server is running
        /// </summary>
        /// <returns></returns>
        [OpenApiOperation("Running", "Returns 200 if the server is running")]
        [HttpGet("running")]
        public ActionResult Running()
        {
            return Ok(GetOkResponse());
        }

        /// <summary>
        /// Returns 200 if the server is ready to process requests
        /// </summary>
        /// <returns></returns>
        [OpenApiOperation("Ready", "Returns 200 if the server is ready to process requests")]
        [HttpGet("ready")]
        public ActionResult Ready()
        {
            return Ok(GetOkResponse());
        }

        private PingResponse GetOkResponse() => new PingResponse
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",
            Status = "Running",
        };
    }
}
