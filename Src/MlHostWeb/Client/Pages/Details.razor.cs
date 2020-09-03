using Microsoft.AspNetCore.Components;
using MlHostWeb.Client.Application.Menu;
using MlHostWeb.Client.Services;
using MlHostWeb.Shared;

namespace MlHostWeb.Client.Pages
{
    public partial class Details : ComponentBase
    {
        [Inject]
        public ModelConfiguration ModelConfiguration { get; set; }

        [Inject]
        public NavMenuService NavMenuService { get; set; }

        public ModelItem ModelItem { get; private set; }

        [Parameter]
        public string VersionId { get; set; }

        protected override void OnParametersSet()
        {
            ModelItem = ModelConfiguration.GetModel(VersionId);

            //NavMenuService.BreadcrumbItems = new[]
            //{
            //    new BreadcrumbItem { Text = ModelItem.Name, Href = $"model/{VersionId}" },
            //    new BreadcrumbItem { Text = "Details" }
            //};

            NavMenuService.PageMenuItems = new IMenuItem[]
            {
                new MenuItem("Overview", $"model/{VersionId}", "oi-list-rich"),
                new MenuItem("Try It", $"tryit/{VersionId}", "oi-aperture"),
            };

            base.OnParametersSet();
        }
    }
}
