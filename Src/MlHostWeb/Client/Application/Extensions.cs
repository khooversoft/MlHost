using Microsoft.JSInterop;
using MlHostSdk.Models;
using MlHostWeb.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHostWeb.Client.Application
{
    internal static class Extensions
    {
        public static IReadOnlyList<Intent> GetIntents(this PredictResponse subject)
        {
            subject.VerifyNotNull(nameof(subject));

            return (subject.Intents ?? Array.Empty<Intent>())
                .OrderByDescending(x => x.Score)
                .ToList();
        }

        public static IReadOnlyList<Intent> GetTopIntents(this PredictResponse subject)
        {
            return subject.GetIntents()
                .Take(5)
                .ToList();
        }

        public static async Task OpenSwagger(this IJSRuntime iJSRuntime, ModelItem modelItem) =>
            await iJSRuntime.InvokeAsync<object>("open", modelItem.GetSwaggerUrl(), "_blank");
    }
}
