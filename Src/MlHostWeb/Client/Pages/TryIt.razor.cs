using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MlHostApi.Models;
using MlHostWeb.Client.Application.Menu;
using MlHostWeb.Client.Application.Models;
using MlHostWeb.Client.Services;
using MlHostWeb.Shared;
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


        protected override void OnParametersSet()
        {
            ModelItem = ModelConfiguration.GetModel(VersionId);
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
                    ModelInput.Result = $"Returned StatusCode={httpResponseMessage.StatusCode}, content={contentJson}";
                    return;
                }

                PredictResponse predictResponse = Json.Deserialize<PredictResponse>(contentJson);

                ModelInput.Result = Json.SerializeWithIndent(predictResponse);
            }
            catch
            {
                ModelInput.Result = $"Cannot connect to {ModelItem.ModelUrl}";
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
    }
}
