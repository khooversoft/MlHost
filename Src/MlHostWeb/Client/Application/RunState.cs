using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Application
{
    public enum RunState
    {
        Startup,
        Message,
        Result,
        Error
    }
}
