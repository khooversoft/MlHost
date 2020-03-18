using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MlHost.Models;
using MlHost.Services;

namespace MlHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly IExecutionContext _executionContext;

        public PingController(IExecutionContext executionContext)
        {
            _executionContext = executionContext;
        }

        [HttpGet]
        public IActionResult Ping()
        {
            var response = new PingResponse
            {
                Status = _executionContext.Running ? "Running" : "Starting up",
            };

            return Ok(response);
        }
    }
}