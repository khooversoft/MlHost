using System;
using System.Collections.Generic;
using System.Text;

namespace MlProcess.Services
{
    [Serializable]
    public class CircuitBreakerException : Exception
    {
        public CircuitBreakerException() { }
        public CircuitBreakerException(string message) : base(message) { }
        public CircuitBreakerException(string message, Exception inner) : base(message, inner) { }
        protected CircuitBreakerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
