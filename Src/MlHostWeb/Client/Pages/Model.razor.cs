﻿using Microsoft.AspNetCore.Components;
using MlHostWeb.Client.Application.Menu;
using MlHostWeb.Client.Application.Models;
using MlHostWeb.Client.Services;
using MlHostWeb.Shared;

namespace MlHostWeb.Client.Pages
{
    public partial class Model : ComponentBase
    {
        [Inject]
        public ModelConfiguration ModelConfiguration { get; set; }

        [Inject]
        public NavMenuService NavMenuService { get; set; }

        public ModelItem ModelItem { get; private set; }

        public ModelQuestion ModelInput { get; set; } = new ModelQuestion();

        [Parameter]
        public string VersionId { get; set; }

        protected override void OnParametersSet()
        {
            ModelItem = ModelConfiguration.GetModel(VersionId);

            //NavMenuService.BreadcrumbItems = new[] { new BreadcrumbItem { Text = ModelItem.Name, Href = $"model/{VersionId}" } };

            NavMenuService.PageMenuItems = new IMenuItem[]
            {
                new MenuItem("Try It", $"tryit/{VersionId}", "oi-aperture"),
                new MenuItem("Details", $"details/{VersionId}", "oi-list"),
            };

            base.OnParametersSet();
        }
    }
}
