using MlHostWeb.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Server.Application
{
    public class Option : IOption
    {
        public string Environment { get; set; } = "dev";

        public string? ConfigFile { get; set; }

        public IList<ModelItem> Models { get; set; } = null!;
    }
}
