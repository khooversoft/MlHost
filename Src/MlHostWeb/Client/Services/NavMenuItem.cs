using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Services
{
    public class NavMenuItem
    {
        public NavMenuItem(string text, string href, string iconName)
        {
            Text = text;
            Href = href;
            IconName = iconName;
        }

        public string Text { get; }
        public string Href { get; }
        public string IconName { get; }
    }
}
