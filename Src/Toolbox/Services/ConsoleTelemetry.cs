using System;
using Toolbox.Tools;

namespace Toolbox.Services
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
