using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHost.Tools
{
    internal static class ActivityExtensions
    {
        public static async Task RunActivities(this IEnumerable<Activity> activites, CancellationToken token, ILogger logger)
        {
            activites.VerifyNotNull(nameof(activites));
            logger.VerifyNotNull(nameof(logger));

            foreach (var activity in activites)
            {
                if (token.IsCancellationRequested) return;

                await runActivity(activity);
            }

            async Task runActivity(Activity activity)
            {
                try
                {
                    logger.LogInformation($"[Activity] Starting {activity.Description}");
                    await activity.Execute();

                    logger.LogInformation($"[Activity] Completed {activity.Description}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"[Activity] {activity.Description} failed");
                    throw;
                }
            }
        }
    }
}
