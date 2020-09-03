﻿using Microsoft.Extensions.Logging;
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
        public NavMenuService() { }

        public IReadOnlyList<MenuItem> GetLeftMenuItems() => new[]
        {
            new MenuItem("Home", string.Empty, "oi-home"),

            new MenuItem("ML Models", "oi-layers", "mlmodels", new []
            {
                new MenuItem("Intent", "model/intent-v1", "oi-list-rich"),
                new MenuItem("Emotion", "model/emotion-v2", "oi-list-rich"),
                new MenuItem("Sentiment", "model/sentiment-v2", "oi-list-rich"),
                new MenuItem("ID Card", "model/idcard-v1", "oi-list-rich"),
            }),

            new MenuItem("BOT", string.Empty, "oi-fork"),
        };

        //public IReadOnlyList<BreadcrumbItem> BreadcrumbItems { get; set; }

        public IReadOnlyList<IMenuItem> PageMenuItems { get; set; }
    }
}
