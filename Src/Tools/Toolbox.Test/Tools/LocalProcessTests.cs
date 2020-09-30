//using FluentAssertions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Toolbox.Application;
//using Toolbox.Tools;
//using Toolbox.Tools.Local;

//namespace Toolbox.Test.Tools
//{
//    [TestClass]
//    public class LocalProcessTests
//    {
//        [TestMethod]
//        public async Task GivenCommand_WhenExecuted_ShouldReturnCorrectResult()
//        {
//            var logger = new MemoryLogger();
//            var token = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;

//            var localProcess = new LocalProcessBuilder()
//            {
//                ExecuteFile = "pwsh.exe",
//                Arguments = "-Command \"& {Get-Location}\"",
//            }.Build(logger);

//            await localProcess.Run(token);
//            logger.Count().Should().BeGreaterThan(0);

//            localProcess.ExitCode.Should().Be(0);
//        } 
        
//        [TestMethod]
//        public async Task GivenBadCommand_WhenExecuted_ShouldThrow()
//        {
//            var logger = new MemoryLogger();
//            var token = new CancellationTokenSource(TimeSpan.FromSeconds(10000)).Token;

//            var localProcess = new LocalProcessBuilder()
//            {
//                ExecuteFile = "pwsh_bad.exe",
//                Arguments = "-Command \"& {Get-Location}\"",
//            }.Build(logger);

//            Func<Task> act = () => localProcess.Run(token);
//            await act.Should().ThrowAsync<InvalidOperationException>();
//            logger.Count().Should().BeGreaterThan(0);
//        }

//        [TestMethod]
//        public async Task GivenSleepCommand_WhenExecuted_ShouldPass()
//        {
//            var logger = new MemoryLogger();
//            var token = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;

//            var localProcess = new LocalProcessBuilder()
//                .SetExecuteFile("pwsh.exe")
//                .SetArguments("-Command \"& {Start-Sleep 2}\"")
//                .Build(logger);

//            await localProcess.Run(token);
//            logger.Count().Should().BeGreaterThan(0);

//            localProcess.ExitCode.Should().Be(0);
//        }

//        [TestMethod]
//        public async Task GivenExitCode_WhenExecuted_ShouldPassExpected()
//        {
//            var logger = new MemoryLogger();
//            var token = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;

//            var localProcess = new LocalProcessBuilder()
//                .SetExecuteFile("pwsh.exe")
//                .SetSuccessExitCode(1)
//                .SetArguments("-Command \"Exit(1);")
//                .Build(logger);

//            await localProcess.Run(token);
//            logger.Count().Should().BeGreaterThan(0);

//            localProcess.ExitCode.Should().Be(1);
//        }

//        [TestMethod]
//        public async Task GivenExitCode_WhenDifferentExitCode_ShouldPassExpected()
//        {
//            var logger = new MemoryLogger();
//            var token = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;

//            var localProcess = new LocalProcessBuilder()
//                .SetExecuteFile("pwsh.exe")
//                .SetSuccessExitCode(1)
//                .SetArguments("-Command \"Exit(2);")
//                .Build(logger);

//            LocalProcess? subject = null;
//            Func<Task<LocalProcess>> act = async () => subject = await localProcess.Run(token);
//            await act.Should().ThrowAsync<ArgumentException>();

//            subject.Should().BeNull();
//            logger.Count().Should().BeGreaterThan(0);

//            localProcess.ExitCode.Should().Be(2);
//        }

//        [TestMethod]
//        public async Task GivenSleepOverTimeoutCommandWithCancellationToken_WhenExecuted_ShoulNotThrowExection()
//        {
//            var logger = new MemoryLogger();
//            var token = new CancellationTokenSource(TimeSpan.FromSeconds(4)).Token;

//            var localProcess = new LocalProcessBuilder()
//                .SetExecuteFile("pwsh.exe")
//                .SetArguments("-Command \"& {Start-Sleep 10}\"")
//                .Build(logger);

//            LocalProcess? subject = null;
//            Func<Task<LocalProcess>> act = async () => subject = await localProcess.Run(token);
//            await act.Should().NotThrowAsync();

//            subject.Should().NotBeNull();
//            subject!.IsRunning.Should().BeFalse();
//        }
//    }
//}
