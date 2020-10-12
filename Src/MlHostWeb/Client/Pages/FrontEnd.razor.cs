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
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Security.Cryptography.X509Certificates;
using Toolbox.Tools;

namespace MlHostWeb.Client.Pages
{
    public partial class FrontEnd : ComponentBase
    {
        private const string _stateId = "default";

        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public HostConfigurationService ModelConfiguration { get; set; }

        [Inject]
        public StateCacheService StateCacheService { get; set; }

        public bool IsExecuting { get; set; }

        public RunContext Context { get; set; }

        public static KeyValuePair<string, string>[] IntentLimitList { get; } = Enumerable
            .Range(0, 10)
            .Select(x => new KeyValuePair<string, string>(x.ToString(), x == 0 ? "All" : x.ToString()))
            .ToArray();

        protected override void OnParametersSet()
        {
            Context = StateCacheService.GetOrCreate(_stateId, () => new RunContext(_stateId));

            Context.Models = ModelConfiguration
                .GetModels()
                .Select(x => new ModelSelect(x.Name))
                .ToArray();
        }

        protected async Task SubmitForm()
        {
            if (Context.Request.IsEmpty())
            {
                Context.SetError("Request is required");
                StateHasChanged();
                return;
            }

            if (!Context.Models.Any(x => x.Checked))
            {
                Context.SetError("One or more models must be selected to execute against");
                StateHasChanged();
                return;
            }

            int intentLimit = int.TryParse(Context.IntentLimitId, out int intentLimitValue) ? intentLimitValue : 0;

            var request = new BatchRequest
            {
                Request = Context.Request,
                Models = Context.Models
                    .Where(x => x.Checked)
                    .Select(x => new ModelRequest { ModelName = x.Name, IntentLimit = intentLimit <= 0 ? (int?)null : intentLimit })
                    .ToList(),
            };

            try
            {
                IsExecuting = true;
                StateHasChanged();

                PostResponse<BatchResponse> response = await ModelConfiguration
                    .GetFrontEndApi()
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

                Context.SetResult(response.Value.Responses, Json.Default.SerializeWithIndent(response.Value));
            }
            catch
            {
                Context.SetError($"Cannot connect to {ModelConfiguration.GetFrontEndApi().ApiUrl.GetRequestUrl()}");
                return;
            }
            finally
            {
                IsExecuting = false;
                StateHasChanged();
            }
        }

        public class RunContext : RunContextBase
        {
            private readonly string _stateId;

            public RunContext(string modelName) => _stateId = modelName;

            [Required]
            public string Request { get; set; }

            public string IntentLimitId { get; set; }

            public string Result { get; set; }

            public ModelSelect[] Models { get; set; }

            public IList<int> CheckValues { get; set; } = new List<int>();

            public PredictResponse[] Responses { get; set; }

            public void SetResult(IEnumerable<PredictResponse> responses, string result)
            {
                Clear();
                RunState = RunState.Result;
                Result = result;
                Responses = responses.ToArray();
            }

            public override string GetId() => _stateId;

            protected override void ClearState()
            {
                Result = null;
                Responses = null;
            }
        }

        public class ModelSelect
        {
            public ModelSelect(string name)
            {
                Name = name;
            }

            public string Name { get; }
            public bool Checked { get; set; }
        }
    }
}
