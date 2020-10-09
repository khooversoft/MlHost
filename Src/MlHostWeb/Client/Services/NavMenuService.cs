using Microsoft.Extensions.Logging;
using MlHostWeb.Client.Application;
using MlHostWeb.Client.Application.Menu;
using MlHostWeb.Client.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Services
{
    public class NavMenuService
    {
        private readonly ModelConfiguration _modelConfiguration;

        public NavMenuService(ModelConfiguration modelConfiguration)
        {
            _modelConfiguration = modelConfiguration;
        }

        public IReadOnlyList<MenuItem> GetLeftMenuItems() => new[]
        {
            new MenuItem("Home", string.Empty, "oi-home", true),

            new MenuItem(
                "ML Models", 
                "oi-layers", 
                "mlmodels",
                _modelConfiguration.GetModels().Select(x => new MenuItem(x.Name, $"model/{x.Name}", "oi-list-rich", true)).ToArray()
                ),

            //new MenuItem("ML Models", "oi-layers", "mlmodels", new []
            //{
            //    new MenuItem("Intent", "model/intent-v1", "oi-list-rich", true),
            //    new MenuItem("Emotion", "model/emotion-v2", "oi-list-rich", true),
            //    new MenuItem("Sentiment", "model/sentiment-v2", "oi-list-rich", true),
            //    new MenuItem("ID Card", "model/idcard-v1", "oi-list-rich", true),
            //}),

            new MenuItem("BOT", string.Empty, "oi-fork", true),
        };
    }
}
