using Microsoft.VisualStudio.TestPlatform;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Toolbox.Tools;

namespace MlFrontEnd.Test.Application
{
    internal class TestHttpClientFactory : IHttpClientFactory
    {
        private readonly ConcurrentDictionary<string, TestFakeMlHost> _hosts = new ConcurrentDictionary<string, TestFakeMlHost>(StringComparer.OrdinalIgnoreCase);

        public TestHttpClientFactory(IEnumerable<KeyValuePair<string, TestFakeMlHost>> hosts)
        {
            hosts.VerifyNotNull(nameof(hosts));

            foreach(var host in hosts)
            {
                _hosts.TryAdd(host.Key, host.Value)
                    .VerifyAssert(x => x == true, $"Failed to add {host.Key}");
            }
        }

        public HttpClient CreateClient(string name) => _hosts[name].HttpClient;
    }
}
