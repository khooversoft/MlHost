using Microsoft.AspNetCore.Mvc;
using MlHost.Application;
using MlHost.Services;
using MlHostApi.Models;
using MlHostApi.Types;

namespace MlHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly IExecutionContext _executionContext;
        private readonly IOption _option;

        public PingController(IExecutionContext executionContext, IOption option)
        {
            _executionContext = executionContext;
            _option = option;
        }

        [HttpGet]
        public IActionResult Ping()
        {
            var response = new PingResponse
            {
                Status = _executionContext.State.ToString(),
                ModelId = _executionContext?.ModelId?.ToString(),
                HostName = _option.HostName,
            };

            return Ok(response);
        }
    }
}