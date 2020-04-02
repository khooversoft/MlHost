using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Application;
using Toolbox.Tools;

namespace Toolbox.Test.Tools
{
    [TestClass]
    public class LocalProcessTests
    {
        [TestMethod]
        public async Task GivenCommand_WhenExecuted_ShouldReturnCorrectResult()
        {
            var logger = new MemoryLogger();
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;

            var localProcess = new LocalProcess(logger)
            {
                File = "powershell.exe",
                Arguments = "-Command \"& {Get-Location}\"",
            };

            await localProcess.Run(token);
            logger.Count().Should().BeGreaterThan(0);

            localProcess.ExitCode.Should().Be(0);
        }

        [TestMethod]
        public async Task GivenSleepCommand_WhenExecuted_ShouldPass()
        {
            var logger = new MemoryLogger();
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;

            var localProcess = new LocalProcess(logger)
            {
                File = "powershell.exe",
                Arguments = "-Command \"& {Start-Sleep 2}\"",
            };

            await localProcess.Run(token);
            logger.Count().Should().BeGreaterThan(0);

            localProcess.ExitCode.Should().Be(0);
        }

        [TestMethod]
        public async Task GivenSleepOverTimeoutCommand_WhenExecuted_ShouldThrowExection()
        {
            var logger = new MemoryLogger();
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token;

            var localProcess = new LocalProcess(logger)
            {
                File = "powershell.exe",
                Arguments = "-Command \"& {Start-Sleep 10}\"",
            };

            Func<Task> act = () => localProcess.Run(token);
            await act.Should().NotThrowAsync();
        }
    }
}
