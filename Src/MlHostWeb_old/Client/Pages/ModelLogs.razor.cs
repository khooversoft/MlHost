using Microsoft.AspNetCore.Components;
using MlHostWeb.Client.Application;
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

        public RunContext Context { get; set; }


        protected override void OnParametersSet()
        {
            ModelItem = ModelConfiguration.GetModel(VersionId);
            Context = new RunContext();

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
                Context.SetMessages(pingLogs.Count, pingLogs.Messages);
            }
            catch
            {
                Context.SetError($"Cannot connect to {ModelItem.LogUrl}");
            }
            finally
            {
                IsExecuting = false;
                StateHasChanged();
            }
        }

        public class RunContext
        {
            public RunState RunState { get; private set; }

            public int? Count { get; private set; }

            public IList<MessageItem> Messages { get; private set; }

            public string ErrorMessage { get; private set; }

            public void SetMessages(int count, IEnumerable<string> messages)
            {
                Clear();
                RunState = RunState.Result;
                Count = count;
                Messages = messages.Select((x, i) => new MessageItem(i, x)).ToList();
            }

            public void SetError(string errorMessage)
            {
                Clear();
                RunState = RunState.Error;
                ErrorMessage = errorMessage;
            }

            private void Clear()
            {
                RunState = RunState.Startup;
                Count = null;
                Messages = null;
            }

            public class MessageItem
            {
                public MessageItem(int index, string message)
                {
                    Index = index;
                    Message = message;
                }

                public int Index { get; }
                public string Message { get; }
            }
        }

        private class PingLogs
        {
            public int Count { get; set; }

            public IList<string> Messages { get; set; }
        }
    }
}
