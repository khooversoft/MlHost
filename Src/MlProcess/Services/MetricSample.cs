using System;
using System.Collections.Generic;
using System.Text;

namespace MlProcess.Services
{
    internal struct MetricSample
    {
        public MetricSample(string name, TimeSpan span, int count)
        {
            Name = name;
            Span = span;
            Count = count;
        }

        public TimeSpan Span { get; }

        public string Name { get; }

        public int Count { get; }

        public float Tps => Count == 0 || Span.TotalSeconds == 0 ? 0 : Count / (float)Span.TotalSeconds;
    }
}
