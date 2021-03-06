﻿using Microsoft.AspNetCore.Mvc.Formatters;
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
        private static readonly ConcurrentDictionary<string, TestFakeMlHost> _hosts = new ConcurrentDictionary<string, TestFakeMlHost>(StringComparer.OrdinalIgnoreCase);
        private static ILogger? _logger;
        private static TestFrontendHost? _testFrontendHost;
        private static readonly object _lock = new object();

        static TestApplication()
        {
            _logger ??= LoggerFactory.Create(config => config.AddDebug())
                .CreateLogger("test");
        }

        internal static IReadOnlyList<string> ModelNames { get; } = new[]
        {
            "model_1",
            "model_2",
            "model_3",
        };

        [AssemblyCleanup]
        public static void Cleanup()
        {
            Interlocked.Exchange(ref _testFrontendHost, null!)?.Shutdown();

            _hosts.Values
                .ForEach(x => x.Shutdown());

            _hosts.Clear();
        }

        internal static ILogger? CreateLogger() => _logger;

        internal static TestFrontendHost StartHosts()
        {
            lock (_lock)
            {
                if (_testFrontendHost != null) return _testFrontendHost;

                ModelNames
                    .ForEach(x => GetMlHost(x));

                return _testFrontendHost = new TestFrontendHost()
                    .StartApiServer(_hosts.ToList());
            }
        }

        private static void GetMlHost(string modelName)
        {
            modelName.VerifyNotEmpty(nameof(modelName));

            if (_hosts.TryGetValue(modelName, out TestFakeMlHost? testMlHost)) return;

            _logger.LogInformation($"Starting server {modelName}");

            var testHost = new TestFakeMlHost(modelName);

            _hosts.TryAdd(modelName, testHost)
                .VerifyAssert(x => x == true, "Failed to add");

            testHost.StartApiServer();
        }
    }
}
