using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostWeb.Shared
{
    public class Configuration
    {
        public string FrontEndUrl { get; set; }
        public IList<ModelItem> Models { get; set; }
    }
}
