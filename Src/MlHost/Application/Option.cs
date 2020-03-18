using MlHost.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Application
{
    internal class Option : IOption
    {
        public string ServiceUri { get; set; } = "http://localhost:5003/predict";

        public bool ForceDeployment { get; set; } = false;

        public string ZipFileUri { get; set; } = null!;

        public BlobStoreOption BlobStore { get; set; } = null!;

        public DeploymentOption Deployment { get; set; } = null!;

        public void Verify()
        {
            ServiceUri.VerifyNotEmpty($"{nameof(ServiceUri)} is missing");
            BlobStore.VerifyNotNull($"{nameof(BlobStore)} is missing");

            BlobStore.Verify();
            Deployment.Verify();
        }

        public IReadOnlyList<KeyValuePair<string, string>> ToDetails()
        {
            return new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>(nameof(ServiceUri), ServiceUri),
                new KeyValuePair<string, string>(nameof(ForceDeployment), ForceDeployment.ToString()),
            }
            .Concat(BlobStore.ToDetails(nameof(BlobStore)))
            .Concat(Deployment.ToDetails(nameof(Deployment)))
            .ToList();
        }
    }
}
