using MlHostWeb.Shared;
using System.Collections.Generic;
using Toolbox.Application;

namespace MlHostWeb.Server.Application
{
    public interface IOption
    {
        string? ConfigFile { get; }
        string Environment { get; }
        string FrontEndUrl { get; }
        IList<ModelItem> Models { get; }
        RunEnvironment RunEnvironment { get; }
    }
}