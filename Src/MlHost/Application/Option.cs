using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Application
{
    public class Option : IOption
    {
        public string? ServiceUri { get; set; }

        public bool ForceDeployment { get; set; }

        public void Verify()
        {
            if (string.IsNullOrWhiteSpace(ServiceUri)) throw new ArgumentException($"{nameof(ServiceUri)} is missing");
        }
    }
}
