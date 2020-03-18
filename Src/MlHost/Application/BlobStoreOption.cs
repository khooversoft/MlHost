using MlHost.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Application
{
    internal class BlobStoreOption
    {
        public string ContainerName { get; set; } = null!;

        public string ConnectionString { get; set; } = null!;

        public void Verify()
        {
            ContainerName.VerifyNotEmpty($"{nameof(ContainerName)} is missing");
            ConnectionString.VerifyNotEmpty($"{nameof(ConnectionString)} is missing");
        }

        public IReadOnlyList<KeyValuePair<string, string>> ToDetails(string path)
        {
            Func<string, string> fmtKey = x => path + ":" + x;

            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(fmtKey(nameof(ContainerName)), ContainerName),
                new KeyValuePair<string, string>(fmtKey(nameof(ConnectionString)), ConnectionString),
            };
        }
    }
}
