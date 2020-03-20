using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostCli.Application
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
    }
}
