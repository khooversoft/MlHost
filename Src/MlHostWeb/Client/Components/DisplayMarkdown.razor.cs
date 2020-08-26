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
        public IContentService ContentService { get; set; }

        [Parameter]
        public string Id { get; set; }

        public string GetDocHtml() => @ContentService.GetDocHtml(Id);
    }
}
