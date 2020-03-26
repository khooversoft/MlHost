using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostApi.Services
{
    public class ConsoleTelemetry : ITelemetry
    {
        private readonly ISecretFilter _secretFilter;

        public ConsoleTelemetry(ISecretFilter secretFilter)
        {
            secretFilter.VerifyNotNull(nameof(secretFilter));

            _secretFilter = secretFilter;
        }

        public void WriteLine(string? message) => Console.WriteLine(_secretFilter.FilterSecrets(message ?? string.Empty));
    }
}
