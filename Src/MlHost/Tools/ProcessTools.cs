using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Tools
{
    public static class ProcessTools
    {
        public static void KillAnyRunningProcesses(ILogger logger)
        {
            foreach (var process in Process.GetProcessesByName("python"))
            {
                logger.LogWarning($"Killing already running python.exe process {process.ProcessName} before starting child process");

                try
                {
                    process.Kill(true);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Cannot kill process {process.ProcessName}");
                    throw;
                }
            }
        }
    }
}
