using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Application;
using Toolbox.Tools;

namespace Toolbox.Test.Tools
{
    [TestClass]
    public class OsLocalProcessTests
    {
        [TestMethod]
        public async Task GivenCommand_WhenExecuted_ShouldReturnCorrectResult()
        {
            var logger = new MemoryLogger<LocalProcess>();

            LocalProcess localProcess = new LocalProcessBuilder()
            {
                ExecuteFile = "powershell.exe",
                Arguments = "-Command \"& {Get-Location}\"",
            }.Build(logger);

            Process process = await localProcess.Run();
            logger.Count().Should().BeGreaterThan(0);

            process.ExitCode.Should().Be(0);
        }

        [TestMethod]
        public async Task GivenBadCommand_WhenExecuted_ShouldThrow()
        {
            var logger = new MemoryLogger<LocalProcess>();

            LocalProcess localProcess = new LocalProcessBuilder()
            {
                ExecuteFile = "pwsh_bad.exe",
                Arguments = "-Command \"& {Get-Location}\"",
            }.Build(logger);

            Func<Task> act = () => localProcess.Run();
            await act.Should().ThrowAsync<InvalidOperationException>();
            logger.Count().Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task GivenSleepCommand_WhenExecuted_ShouldPass()
        {
            var logger = new MemoryLogger<LocalProcess>();

            LocalProcess localProcess = new LocalProcessBuilder()
                .SetExecuteFile("powershell.exe")
                .SetArguments("-Command \"& {Start-Sleep 2}\"")
                .Build(logger);

            var token = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
            token.Register(() => localProcess.Stop());

            Process process = await localProcess.Run();
            logger.Count().Should().BeGreaterThan(0);

            process.HasExited.Should().BeTrue();
        }

        [TestMethod]
        public async Task GivenExitCode_WhenExecuted_ShouldPassExpected()
        {
            var logger = new MemoryLogger<LocalProcess>();

            LocalProcess localProcess = new LocalProcessBuilder()
                .SetExecuteFile("powershell.exe")
                .SetArguments("-Command \"Exit(1);")
                .Build(logger);

            Process process = await localProcess.Run();
            logger.Count().Should().BeGreaterThan(0);

            process.ExitCode.Should().Be(1);
        }

        [TestMethod]
        public async Task GivenSleepOverTimeoutCommandWithCancellationToken_WhenExecuted_ShoulNotThrowExection()
        {
            var logger = new MemoryLogger<LocalProcess>();

            LocalProcess localProcess = new LocalProcessBuilder()
                .SetExecuteFile("powershell.exe")
                .SetArguments("-Command \"& {Start-Sleep 10}\"")
                .Build(logger);

            var token = new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token;
            token.Register(() => localProcess.Stop());

            using Process? subject = await localProcess.Run();

            subject.Should().NotBeNull();
            subject!.HasExited.Should().BeTrue();
        }

        [TestMethod]
        public async Task GivenCommandWithoutputNoDelayUsingPwsh_WhenCaptured_ShouldSucced()
        {
            var logger = new MemoryLogger<LocalProcess>();
            var captureList = new List<string>();

            var commands = new[]
            {
                "Write-Host 'Starting process';",
                "Write-Host 'Process is running';",
                "Write-Host 'Server running on port 5003';",
            };

            string command = commands.Aggregate("-Command \"& {", (a, x) => a + " " + x) + "}\"";

            LocalProcess localProcess = new LocalProcessBuilder()
                .SetExecuteFile("pwsh.exe")
                .SetArguments(command)
                .SetCaptureOutput(x => captureList.Add(x))
                .Build(logger);

            Process? subject = await localProcess.Run();

            subject.Should().NotBeNull();
            subject!.HasExited.Should().BeTrue();
            captureList.Count.Should().Be(commands.Length);
        }

        [TestMethod]
        public async Task GivenCommandWithoutputNoDelayUsingPowershell_WhenCaptured_ShouldSucced()
        {
            var logger = new MemoryLogger<LocalProcess>();
            var captureList = new List<string>();

            var commands = new[]
            {
                "Write-Host 'Starting process';",
                "Write-Host 'Process is running';",
                "Write-Host 'Server running on port 5003';",
            };

            string command = commands.Aggregate("-Command \"& {", (a, x) => a + " " + x) + "}\"";

            LocalProcess localProcess = new LocalProcessBuilder()
                .SetExecuteFile("powershell.exe")
                .SetArguments(command)
                .SetCaptureOutput(x => captureList.Add(x))
                .Build(logger);

            Process? subject = await localProcess.Run();

            subject.Should().NotBeNull();
            subject!.HasExited.Should().BeTrue();
            captureList.Count.Should().Be(commands.Length);
        }

        [TestMethod]
        public async Task GivenCommandWithoutputWithDelay_WhenCaptured_ShouldSucced()
        {
            var logger = new MemoryLogger<LocalProcess>();
            var captureList = new List<string>();

            var commands = new[]
            {
                "Start-Sleep 2;",
                "Write-Host 'Starting process';",
                "Start-Sleep 2;",
                "Write-Host 'Process is running';",
                "Start-Sleep 2;",
                "Write-Host 'Server running on port 5003';",
                "Start-Sleep 2;",
            };

            string command = commands.Aggregate("-Command \"& {", (a, x) => a + " " + x) + "}\"";

            LocalProcess localProcess = new LocalProcessBuilder()
                .SetExecuteFile("powershell.exe")
                .SetArguments(command)
                .SetCaptureOutput(x => captureList.Add(x))
                .Build(logger);

            Process? subject = await localProcess.Run();

            subject.Should().NotBeNull();
            subject!.HasExited.Should().BeTrue();
            captureList.Count.Should().Be(commands.Length / 2);
        }
    }
}
