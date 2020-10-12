using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MlHostSdk.Models;
using MlHostWeb.Client.Application;
using MlHostWeb.Client.Application.Models;
using MlHostWeb.Client.Services;
using MlHostWeb.Shared;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Toolbox.Services;
using MlHostSdk.RestApi;
using System.ComponentModel.DataAnnotations;
using Toolbox.Tools;

namespace MlHostWeb.Client.Pages
{
    public partial class TryIt : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public HostConfigurationService ModelConfiguration { get; set; }

        [Inject]
        public NavMenuService NavMenuService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public StateCacheService StateCacheService { get; set; }

        [Parameter]
        public string ModelName { get; set; }

        public ModelItem ModelItem { get; private set; }

        public bool IsExecuting { get; set; }

        public RunContext Context { get; set; }

        protected override void OnParametersSet()
        {
            ModelItem = ModelConfiguration.GetModel(ModelName);
            Context = StateCacheService.GetOrCreate(ModelName, () => new RunContext(ModelName));
        }

        public async Task ShowSwagger() => await JsRuntime.OpenSwagger(ModelItem);

        protected async Task SubmitForm()
        {
            if (Context.Request.IsEmpty())
            {
                Context.SetError("Request is required");
                StateHasChanged();
                return;
            }

            var request = new PredictRequest
            {
                Request = Context.Request,
            };

            try
            {
                IsExecuting = true;
                StateHasChanged();

                PostResponse<PredictResponse> response = await ModelConfiguration
                    .GetModelApi(ModelName)
                    .PostRequest(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.Starting)
                    {
                        Context.SetMessage("Service is starting, please wait and try again.  You can see more details in 'Log'");
                    }
                    else
                    {
                        Context.SetError($"Returned StatusCode={response.StatusCode}, content={response.Content}");
                    }
                }

                Context.SetResult(response.Value, Json.Default.SerializeWithIndent(response.Value));
            }
            catch
            {
                Context.SetError($"Cannot connect to {ModelConfiguration.GetModelApi(ModelName).ApiUrl.GetRequestUrl()}");
                return;
            }
            finally
            {
                IsExecuting = false;
                StateHasChanged();
            }
        }

        protected async Task SaveDataToCsv()
        {
            var data = Context.Response.GetIntents()
                .Aggregate("Label,Score" + Environment.NewLine, (a, x) => a += $"{x.Label},{x.Score}{Environment.NewLine}");

            await JsRuntime.InvokeAsync<object>("FileSaveAs", "mlresponse.csv", data);
        }

        public class RunContext : RunContextBase
        {
            private readonly string _modelName;

            public RunContext(string modelName) => _modelName = modelName;

            [Required]
            public string Request { get; set; }

            public string Result { get; set; }

            public PredictResponse Response { get; set; }

            public void SetResult(PredictResponse response, string result)
            {
                Clear();
                RunState = RunState.Result;
                Result = result;
                Response = response;
            }

            public override string GetId() => _modelName;

            protected override void ClearState()
            {
                Result = null;
                Response = null;
            }
        }
    }
}
