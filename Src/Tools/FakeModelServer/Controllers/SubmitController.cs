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
    public class SubmitController : ControllerBase
    {
        private static Intent[] _intents = Enumerable.Range(0, 10)
            .Select(x => new Intent
            {
                Label = $"Label_{x}",
                Score = 1.0f - (Math.Sqrt(x) / 10)
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
                Intents = _intents,
            };
        }
    }
}
