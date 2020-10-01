using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlHost.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MlHost.Test.Application
{
    [TestClass]
    public class TestApplication
    {
        private static TestWebsiteHost? _currentHost;
        private static SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        internal static async Task<TestWebsiteHost> GetHost()
        {
            await _lock.WaitAsync();

            try
            {
                return _currentHost ??= new TestWebsiteHost().StartApiServer();
            }
            finally
            {
                _lock.Release();
            }
        }

        [AssemblyCleanup]
        public static void Cleanup() => Interlocked.Exchange(ref _currentHost, null!)?.Shutdown();

    }
}
