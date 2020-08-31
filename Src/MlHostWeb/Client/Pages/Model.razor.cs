using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MlHostWeb.Client.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MlHostApi.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using MlHostWeb.Client.Application.Models;
using Toolbox.Services;
using Microsoft.JSInterop;
using MlHostWeb.Shared;

namespace MlHostWeb.Client.Pages
{
    public partial class Model : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public IJson Json { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ModelConfiguration ModelConfiguration { get; set; }

        public ModelItem ModelItem { get; private set; }

        public ModelQuestion ModelInput { get; set; } = new ModelQuestion();

        [Parameter]
        public string VersionId { get; set; }

        protected override void OnParametersSet()
        {
            Console.WriteLine($"{nameof(Model)}:{nameof(OnParametersSet)}, ModelItem.DocId {nameof(ModelItem.DocId)}");

            ModelItem = ModelConfiguration.GetModel(VersionId);
            base.OnParametersSet();
        }

        protected async Task SubmitForm()
        {
            Console.WriteLine($"{nameof(Model)}:{nameof(SubmitForm)} - ModelItem={(ModelItem != null ? "OK (notNull)" : "null")}");

            var request = new ModelRequest
            {
                Sentence = ModelInput.Question,
            };

            Console.WriteLine($"Question: {ModelInput.Question}");

            HttpResponseMessage httpResponseMessage = await Http.PostAsJsonAsync(ModelItem.ModelUrl, request);

            if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ModelInput.Result = $"Returned StatusCode={httpResponseMessage.StatusCode}";
                return;
            }

            string contentJson = await httpResponseMessage.Content.ReadAsStringAsync();
            PredictResponse predictResponse = Json.Deserialize<PredictResponse>(contentJson);

            ModelInput.Result = Json.SerializeWithIndent(predictResponse);
        }

        public async Task ShowSwagger()
        {
            await JsRuntime.InvokeAsync<object>("open", ModelItem.SwaggerUrl, "_blank");
        }

        private class ModelRequest
        {
            public string Sentence { get; set; }
        }
    }
}
