using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Application.Menu
{
    public class MenuItem : IMenuItem
    {
        public MenuItem(string text, string href, string iconName)
        {
            Text = text;
            Href = href;
            IconName = iconName;
        }

        public MenuItem(string text, string iconName, string href, MenuItem[] children)
        {
            Text = text;
            Href = href;
            IconName = iconName;
            Children = children;
        }

        public string Text { get; }
        public string Href { get; }
        public string IconName { get; }

        public MenuItem[] Children { get; set; }
    }
}
