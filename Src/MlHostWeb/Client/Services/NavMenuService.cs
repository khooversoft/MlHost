using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Services
{
    public class NavMenuService
    {
        public NavMenuService() { }

        public IReadOnlyList<NavMenuItem> GetItems() => new[]
        {
            //new NavMenuItem("Models", "models", "oi-layers"),
            new NavMenuItem("Intent", "model/intent-v1", "oi-list-rich"),
            new NavMenuItem("Emotion", "model/emotion-v2", "oi-list-rich"),
            new NavMenuItem("Sentiment", "model/sentiment-v2", "oi-list-rich"),
            new NavMenuItem("ID Card", "model/idcard-v1", "oi-list-rich"),
        };
    }
}
