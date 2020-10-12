using MlHostWeb.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Application;

namespace MlHostWeb.Server.Application
{
    public class Option : IOption
    {
        public string? ConfigFile { get; set; }
        public string Environment { get; set; } = "dev";
        public string FrontEndUrl { get; set; } = null!;
        public IList<ModelItem> Models { get; set; } = null!;
        public RunEnvironment RunEnvironment { get; set; }
    }
}
