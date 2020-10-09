using System.Collections.Generic;
using Toolbox.Application;

namespace MlFrontEnd.Application
{
    public interface IOption
    {
        string? ApplicationUrl { get; }
        string Environment { get; }
        IList<HostOption> Hosts { get; }
        RunEnvironment RunEnvironment { get; }
    }
}