using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Application.Menu
{
    public class MenuCollection : IEnumerable<IMenuItem>
    {
        public List<IMenuItem> MenuItems { get; set; } = new List<IMenuItem>();

        public void Add(IMenuItem menuItem) => MenuItems.Add(menuItem);

        public IEnumerator<IMenuItem> GetEnumerator() => MenuItems.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => MenuItems.GetEnumerator();
    }
}
