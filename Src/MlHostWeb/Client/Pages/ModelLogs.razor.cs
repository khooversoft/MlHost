using Microsoft.AspNetCore.Components;
using MlHostWeb.Client.Application.Menu;
using MlHostWeb.Client.Services;
using MlHostWeb.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Toolbox.Services;

namespace MlHostWeb.Client.Pages
{
    public partial class ModelLogs : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public IJson Json { get; set; }

        [Inject]
        public ModelConfiguration ModelConfiguration { get; set; }

        [Parameter]
        public string VersionId { get; set; }

        public ModelItem ModelItem { get; private set; }

        public bool IsExecuting { get; set; }

        public string Result { get; set; }


        protected override void OnParametersSet()
        {
            ModelItem = ModelConfiguration.GetModel(VersionId);

            base.OnParametersSet();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await GetLogs();
            }
        }

        protected async Task GetLogs()
        {
            try
            {
                IsExecuting = true;
                StateHasChanged();

                PingLogs pingLogs = await Http.GetFromJsonAsync<PingLogs>(ModelItem.LogUrl);

                Result = Json.SerializeWithIndent(pingLogs);
            }
            catch
            {
                Result = $"Cannot connect to {ModelItem.LogUrl}";
                return;
            }
            finally
            {
                IsExecuting = false;
                StateHasChanged();
            }
        }

        private class PingLogs
        {
            public int Count { get; set; }

            public IList<string> Messages { get; set; }
        }
    }
}
