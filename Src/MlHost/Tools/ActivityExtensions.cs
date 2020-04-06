using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHost.Tools
{
    internal static class ActivityExtensions
    {
        public static async Task<bool> RunActivities(this IEnumerable<Activity> activites, CancellationToken token, ILogger logger)
        {
            activites.VerifyNotNull(nameof(activites));
            logger.VerifyNotNull(nameof(logger));

            foreach (var activity in activites)
            {
                if (!token.IsCancellationRequested) return false;

                if (!(await runActivity(activity))) return false;
            }

            return true;

            async Task<bool> runActivity(Activity activity)
            {
                try
                {
                    logger.LogInformation($"[Activity] Starting {activity.Description}");
                    bool success = await activity.Execute();

                    logger.LogInformation($"[Activity] Completed {activity.Description}, status={success}");
                    return success;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"[Activity] {activity.Description} failed");
                    return false;
                }
            }
        }
    }
}
