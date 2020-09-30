using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeModelServer.Application
{
    public class Option : IOption
    {
        public int Port { get; set; } = 5003;
    }
}
