using Microsoft.AspNetCore.Components;
using MlHostSdk.Models;
using MlHostSdk.RestApi;
using MlHostWeb.Client.Application;
using MlHostWeb.Client.Application.Menu;
using MlHostWeb.Client.Application.Models;
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
        public ModelConfiguration ModelConfiguration { get; set; }

        [Inject]
        public StateCacheService StateCacheService { get; set; }

        [Parameter]
        public string ModelName { get; set; }

        public ModelItem ModelItem { get; private set; }

        public bool IsExecuting { get; set; }

        public RunContext Context { get; set; }


        protected override void OnParametersSet()
        {@
            ModelItem = ModelConfiguration.GetModel(ModelName);
            Context = StateCacheService.GetOrCreate(ModelName, () => new RunContext(ModelName));

            base.OnParametersSet();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (Context.LogMessages == null)
                {
                    await GetLogs();
                }
            }
        }

        protected async Task GetLogs()
        {
            try
            {
                IsExecuting = true;
                StateHasChanged();

                PingLogs pingLogs = await ModelConfiguration.GetModelApi(ModelName).GetMlLogs();
                Context.SetLogMessages(pingLogs.Count, pingLogs.Messages);
            }
            catch (Exception ex)
            {
                Context.SetError($"Cannot connect to {ModelItem.ModelUrl}, {ex}");
            }
            finally
            {
                IsExecuting = false;
                StateHasChanged();
            }
        }

        public class RunContext : RunContextBase
        {
            private readonly string _modelName;

            public RunContext(string modelName) => _modelName = modelName;

            public int? Count { get; private set; }

            public IList<LogMessageItem> LogMessages { get; private set; }

            public void SetLogMessages(int count, IEnumerable<string> messages)
            {
                Clear();
                RunState = RunState.Result;
                Count = count;
                LogMessages = messages.Select((x, i) => new LogMessageItem(i, x)).ToList();
            }

            public override string GetId() => _modelName;

            protected override void ClearState()
            {
                Count = null;
                LogMessages = null;
            }

            public class LogMessageItem
            {
                internal LogMessageItem(int index, string message)
                {
                    Index = index;
                    Message = message;
                }

                public int Index { get; }
                public string Message { get; }
            }
        }
    }
}
