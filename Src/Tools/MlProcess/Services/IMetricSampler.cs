using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MlProcess.Services
{
    internal interface IMetricSampler : IDisposable
    {
        void Add(string name);

        void Start();

        void Stop();

        IReadOnlyList<MetricSample> GetSamples();
    }
}