using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FakeMlHost.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MlHostSdk.Models;
using Toolbox.Services;

namespace FakeMlHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmitController : ControllerBase
    {
        private readonly ILogger<SubmitController> _logger;
        private readonly IOption _option;

        private static Intent[] _testResults = new[]
{
            new Intent { Label = "HAPPINESS", Score = 0.9824827 },
            new Intent { Label = "LOVE", Score = 0.009116333 },
        };

        private static Intent[] _fakeIntents = Enumerable.Range(0, 10)
            .Select(x => new Intent
            {
                Label = $"Label_{x}",
                Score = 1.0f - (Math.Sqrt(x + 2) / 10)
            })
            .ToArray();

        public SubmitController(IOption option, ILogger<SubmitController> logger)
        {
            _option = option;
            _logger = logger;
        }

        [HttpPost()]
        public ActionResult<PredictResponse> Submit([FromBody] PredictRequest request)
        {
            _logger.LogInformation($"{nameof(Submit)}: {Json.Default.Serialize(request)}");

            if (!request.IsValidRequest()) return StatusCode((int)HttpStatusCode.BadRequest);

            return new PredictResponse
            {
                Model = new Model
                {
                    Name = _option.VersionId,
                    Version = "1.0"
                },
                Request = request.Request ?? request.Sentence,
                Intents = _testResults
                    .Concat(_fakeIntents)
                    .ToArray(),
            };
        }
    }
}
