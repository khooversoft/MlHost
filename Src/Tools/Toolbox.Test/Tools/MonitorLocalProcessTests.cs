//using FluentAssertions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Toolbox.Application;
//using Toolbox.Tools;
//using Toolbox.Tools.Local;

//namespace Toolbox.Test.Tools
//{
//    [TestClass]
//    public class MonitorLocalProcessTests
//    {
//        [TestMethod]
//        public async Task GivenStartupProcess_WhenExecuted_RestartUntilTimeout()
//        {
//            var logger = new MemoryLogger();
//            var token = new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token;

//            string command = new[]
//            {
//                "Start-Sleep 2;",
//                "Write-Host 'Starting process';",
//                "Start-Sleep 2;",
//                "Write-Host 'Process is running';",
//                "Start-Sleep 2;",
//            }.Aggregate("-Command \"& {", (a, x) => a + " " + x) + "}\"";

//            int runningCount = 0;

//            var tcs = new TaskCompletionSource<bool>();

//            MonitorLocalProcess localProcess = new LocalProcessBuilder()
//            {
//                ExecuteFile = "pwsh.exe",
//                Arguments = command,
//            }
//            .Build(lineData =>
//            {
//                switch (lineData)
//                {
//                    case string s when s.IndexOf("running") >= 0:
//                        Interlocked.Increment(ref runningCount);
//                        tcs.SetResult(true);
//                        return MonitorState.Running;

//                    default:
//                        return null;
//                }
//            }, logger);

//            Task<bool> localTask = localProcess.Start(token);
//            await tcs.Task;

//            bool state = await localTask;
//            state.Should().BeTrue();

//            await localProcess.Completion;
//            await localProcess.Stop();

//            logger.Count().Should().BeGreaterThan(0);
//            runningCount.Should().BeGreaterThan(0);
//            logger.Any(x => x.IndexOf("ExitCode=0") >= 0).Should().BeTrue();
//        }

//        [TestMethod]
//        public async Task GivenStartupProcess_WhenExecuted_SignalsStartThenRestarts()
//        {
//            var logger = new MemoryLogger();
//            var token = new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token;

//            string command = new[]
//            {
//                "Start-Sleep 2;",
//                "Write-Host 'Starting process';",
//                "Start-Sleep 2;",
//                "Write-Host 'Process is running';",
//                "Start-Sleep 2;",
//                "Write-Host 'Process is failing memory';",
//                "Start-Sleep 2;",                                   // Process must still be running to be canceled after detecting failure
//            }.Aggregate("-Command \"& {", (a, x) => a + " " + x) + "}\"";

//            int runningCount = 0;
//            const int maxRetry = 2;
//            var retryPolicy = new MonitorRetryPolicy(new MonitorRetryPolicyConfig(maxRetry, null));
//            var tcs = new TaskCompletionSource<bool>();

//            MonitorLocalProcess monitorLocalProcess = new LocalProcessBuilder()
//            {
//                ExecuteFile = "pwsh.exe",
//                Arguments = command,
//            }
//            .Build(x =>
//            {
//                switch (x)
//                {
//                    case string s when s.IndexOf("running") >= 0:
//                        runningCount++;
//                        tcs.SetResult(true);
//                        return MonitorState.Running;

//                    case string s when s.IndexOf("memory") >= 0:
//                        return retryPolicy.CanRetry() ? MonitorState.Restart : MonitorState.Failed;

//                    default:
//                        return null;
//                }
//            }, logger);


//            Task<bool> localTask = monitorLocalProcess.Start(token);
//            await tcs.Task;

//            bool state = await localTask;
//            state.Should().BeTrue();

//            await monitorLocalProcess.Stop();

//            runningCount.Should().BeGreaterThan(0);
//            logger.Count().Should().BeGreaterThan(0);
//            logger.Where(x => x.IndexOf("ExitCode=-1") >= 0).Count().Should().BeGreaterThan(0);
//            logger.ForEach(x => Console.WriteLine(x));
//        }
//    }
//}
