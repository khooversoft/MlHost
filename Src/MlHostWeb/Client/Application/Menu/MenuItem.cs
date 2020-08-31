using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Application.Menu
{
    public class MenuItem : IMenuItem
    {
        public string Text { get; set; }
        public string Href { get; set; }
    }
}
