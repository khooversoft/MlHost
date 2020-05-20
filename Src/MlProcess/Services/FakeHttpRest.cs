using MlHostApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MlProcess.Services
{
    internal class FakeHttpRest : IHttpRest
    {
        public async Task<PredictResponse> Invoke(string uri, string question, CancellationToken token)
        {
            string hostName = new Uri(uri).Host;

            PredictResponse response = new PredictResponse
            {
                Intents = new List<Intent>
                {
                    new Intent
                    {
                        Label = $"{hostName}_Label1_thread_{Thread.CurrentThread.ManagedThreadId}",
                        Score = 0.98,
                    },
                    new Intent
                    {
                        Label = $"{hostName}_Label2_thread",
                        Score = 0.78,
                    }
                }
            };

            await Task.Delay(TimeSpan.FromSeconds(1));

            return response;
        }
    }
}
