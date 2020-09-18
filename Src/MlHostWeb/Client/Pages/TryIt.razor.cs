using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MlHostApi.Models;
using MlHostWeb.Client.Application.Menu;
using MlHostWeb.Client.Application.Models;
using MlHostWeb.Client.Services;
using MlHostWeb.Shared;
using Radzen;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Toolbox.Services;

namespace MlHostWeb.Client.Pages
{
    public partial class TryIt : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public IJson Json { get; set; }

        [Inject]
        public ModelConfiguration ModelConfiguration { get; set; }

        [Inject]
        public NavMenuService NavMenuService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public string VersionId { get; set; }

        public ModelItem ModelItem { get; private set; }

        public ModelQuestion ModelInput { get; set; } = new ModelQuestion();

        public bool IsExecuting { get; set; }

        public RunContext Context { get; set; }

        protected override void OnParametersSet()
        {
            ModelItem = ModelConfiguration.GetModel(VersionId);
            Context = new RunContext();
            base.OnParametersSet();
        }

        public async Task ShowSwagger()
        {
            await JsRuntime.InvokeAsync<object>("open", ModelItem.SwaggerUrl, "_blank");
        }

        protected async Task SubmitForm()
        {
            var request = new ModelRequest
            {
                Sentence = ModelInput.Question,
            };

            try
            {
                IsExecuting = true;
                StateHasChanged();

                HttpResponseMessage httpResponseMessage = await Http.PostAsJsonAsync(ModelItem.ModelUrl, request);

                string contentJson = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (contentJson.IndexOf("start", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Context.SetMessage("Service is starting, please wait and try again.  You can see more details in 'Log'");
                    }
                    else
                    {
                        Context.SetError($"Returned StatusCode={httpResponseMessage.StatusCode}, content={contentJson}");
                    }
                    return;
                }

                PredictResponse predictResponse = Json.Deserialize<PredictResponse>(contentJson);

                Context.SetResult(predictResponse, Json.SerializeWithIndent(predictResponse));
            }
            catch
            {
                Context.SetError($"Cannot connect to {ModelItem.ModelUrl}");
                return;
            }
            finally
            {
                IsExecuting = false;
                StateHasChanged();
            }
        }

        private class ModelRequest
        {
            public string Sentence { get; set; }
        }

        public enum RunState
        {
            Startup,
            Message,
            Result
        }

        public class RunContext
        {
            public RunContext() { }

            public RunState RunState { get; private set; }

            public bool Error { get; private set; }

            public string Message { get; private set; }

            public string Result { get; set; }

            public PredictResponse Response { get; set; }

            public void Start() => RunState = RunState.Startup;

            public void SetResult(PredictResponse response, string result)
            {
                Clear();
                RunState = RunState.Result;
                Result = result;
                Response = response;
            }

            public void SetMessage(string message)
            {
                Clear();
                RunState = RunState.Message;
                Message = message;
            }

            public void SetError(string errorMessage)
            {
                Clear();
                RunState = RunState.Message;
                Error = true;
                Message = errorMessage;
            }

            private void Clear()
            {
                Error = false;
                Message = null;
                Result = null;
                Response = null;
            }
        }
    }
}
