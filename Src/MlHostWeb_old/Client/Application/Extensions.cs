using MlHostSdk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHostWeb.Client.Application
{
    public static class Extensions
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
    }
}
