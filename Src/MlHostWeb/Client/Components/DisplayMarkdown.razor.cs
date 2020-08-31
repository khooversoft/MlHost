using Microsoft.AspNetCore.Components;
using MlHostWeb.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Components
{
    public partial class DisplayMarkdown
    {
        [Inject]
        public ClientContentService ClientContentService { get; set; }

        [Parameter]
        public string Id { get; set; }

        public string Html { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            Html = await ClientContentService.GetDocHtml(Id);
        }
    }
}
