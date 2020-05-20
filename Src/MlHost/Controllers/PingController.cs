﻿using Microsoft.AspNetCore.Mvc;
using MlHost.Application;
using MlHost.Services;
using MlHost.Tools;
using MlHostApi.Models;
using NSwag.Annotations;
using System.Collections.Generic;
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
        [OpenApiIgnore]
        [HttpGet]
        public ActionResult<PingResponse> Ping()
        {
            var response = new PingResponse
            {
                Status = _executionContext.State.ToString(),
            };

            return Ok(response);
        }

        /// <summary>
        /// Get the last 100 logs in reverse order
        /// </summary>
        /// <returns>array of strings</returns>
        [OpenApiIgnore]
        [HttpGet("Logs")]
        public ActionResult<PingLogs> GetLogs()
        {
            IReadOnlyList<string> logs = _telemetryMemory.GetLoggedMessages();

            var response = new PingLogs
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