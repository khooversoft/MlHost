using MlHostWeb.Shared;
using System.Collections.Generic;
using Toolbox.Application;

namespace MlHostWeb.Server.Application
{
    public interface IOption
    {
        string? ApplicationUrl { get; set; }
        string? ConfigFile { get; set; }
        string Environment { get; set; }
        IList<ModelItem> Models { get; set; }
        RunEnvironment RunEnvironment { get; set; }
    }
}