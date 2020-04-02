using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Tools;

namespace MlHostApi.Tools
{
    public class HostConfigurationPath
    {
        public HostConfigurationPath(string path)
        {
            path.VerifyNotEmpty(nameof(path));

            string[] pathVectors = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (pathVectors.Length != 2) throw new ArgumentException($"{path} is invalid");

            int index = 0;
            ModelName = pathVectors[index++];
            VersionId = pathVectors[index++];
        }

        public string Root { get; } = "ml-models";

        public string ModelName { get; }

        public string VersionId { get; }

        public override string ToString()
        {
            return Root + "/" + ModelName + "/" + VersionId;
        }
    }
}
