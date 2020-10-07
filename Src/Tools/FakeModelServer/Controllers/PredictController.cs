using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeModelServer.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MlHostSdk.Models;
using Toolbox.Tools;

namespace FakeModelServer.Controllers
{
    [Route("predict")]
    [ApiController]
    public class PredictController : ControllerBase
    {
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

        private readonly IOption _option;

        public PredictController(IOption option)
        {
            _option = option;
        }

        [HttpPost]
        public PredictResponse Post([FromBody] PredictRequest request)
        {
            return new PredictResponse
            {
                Model = new Model
                {
                    Name = "fakeModel",
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
