using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MlHostSdk.Models;

namespace FakeModelServer.Controllers
{
    [Route("api/[controller]")]
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
                Request = request.Request,
                Intents = _testResults
                    .Concat(_fakeIntents)
                    .ToArray(),
            };
        }
    }
}
