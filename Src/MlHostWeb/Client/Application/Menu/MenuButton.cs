using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Application.Menu
{
    public class MenuButton : IMenuItem
    {
        public MenuButton(string text, Func<Task> onClick, string iconName)
        {
            Text = text;
            OnClick = onClick;
            IconName = iconName;
        }

        public string Text { get; }
        public Func<Task> OnClick { get; }
        public string IconName { get; }
    }
}
