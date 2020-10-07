using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeMlHost.Application
{
    public class Option : IOption
    {
        public int Port { get; set; } = 5010;
        public string VersionId { get; set; } = "DefaultVersionId";
    }
}
