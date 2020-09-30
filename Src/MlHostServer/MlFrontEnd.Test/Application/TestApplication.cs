using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlFrontEnd.Test.Application
{
    [TestClass]
    public static class TestApplication
    {
        private static readonly ConcurrentDictionary<string, TestMlHost> _hosts = new ConcurrentDictionary<string, TestMlHost>(StringComparer.OrdinalIgnoreCase);
        private static TestFrontendHost? _testFrontendHost;
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private static ILogger? _logger;

        static TestApplication()
        {
            _logger ??= LoggerFactory.Create(config => config.AddDebug())
                .CreateLogger("test");
        }

        internal static ILogger? CreateLogger() => _logger;

        internal static async Task<TestFrontendHost> StartHosts(string[] versionIds)
        {
            if (_testFrontendHost != null) return _testFrontendHost;

            versionIds
                .VerifyNotNull(nameof(versionIds))
                .VerifyAssert(x => x.Length > 0, $"{nameof(versionIds)} list is empty");

            await _lock.WaitAsync();

            try
            {
                TestMlHost[] hosts = await Task.WhenAll<TestMlHost>(versionIds.Select(x => GetMlHost(x)));

                IReadOnlyList<KeyValuePair<string, TestMlHost>> hostList = versionIds
                    .Zip(hosts, (o, i) => new KeyValuePair<string, TestMlHost>(o, i))
                    .ToArray();

                //IReadOnlyList<KeyValuePair<string, TestMlHost>> hostList = new[]
                //{
                //    new KeyValuePair<string, TestMlHost>(versionIds[0], null!),
                //};

                return _testFrontendHost = new TestFrontendHost()
                    .StartApiServer(hostList);
            }
            finally
            {
                _lock.Release();
            }
        }

        private static async Task<TestMlHost> GetMlHost(string versionId)
        {
            versionId.VerifyNotEmpty(nameof(versionId));

            if (_hosts.TryGetValue(versionId, out TestMlHost? testMlHost)) return testMlHost;

            _logger.LogInformation($"Starting server {versionId}");

            var testHost = new TestMlHost();
            _hosts.TryAdd(versionId, testHost)
                .VerifyAssert(x => x == true, "Failed to add");

            testHost.StartApiServer();

            _logger.LogInformation($"Waiting for server {versionId} to start");
            await testHost.WaitForStartup();

            return testHost;
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            Interlocked.Exchange(ref _testFrontendHost, null!)?.Shutdown();

            _hosts.Values
                .ForEach(x => x.Shutdown());

            _hosts.Clear();
        }
    }
}
