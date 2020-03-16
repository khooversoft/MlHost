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
            string msg = _executionContext.Running ? "Running" : "Starting up";

            var response = new PingResponse
            {
                Status = msg,
            };

            return Ok(response);
        }
    }
}