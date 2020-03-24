using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostApi.Services
{
    public class ConsoleTelemetry : ITelemetry
    {
        public ConsoleTelemetry() { }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
