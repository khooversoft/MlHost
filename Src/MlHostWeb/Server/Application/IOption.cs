using MlHostWeb.Shared;
using System.Collections.Generic;

namespace MlHostWeb.Server.Application
{
    public interface IOption
    {
        string? ConfigFile { get; set; }
        string Environment { get; set; }
        IList<ModelItem> Models { get; set; }
    }
}