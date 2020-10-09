using System.Collections.Generic;
using Toolbox.Application;

namespace MlFrontEnd.Application
{
    public interface IOption
    {
        string Environment { get; set; }
        IList<HostOption> Hosts { get; set; }
        int? Port { get; set; }
        RunEnvironment RunEnvironment { get; set; }
    }
}