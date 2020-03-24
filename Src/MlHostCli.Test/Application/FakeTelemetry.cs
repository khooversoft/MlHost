using MlHostApi.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostCli.Test.Application
{
    internal class FakeTelemetry : ITelemetry
    {
        public void WriteLine(string message)
        {
        }
    }
}
