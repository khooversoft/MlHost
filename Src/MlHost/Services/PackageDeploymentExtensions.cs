using MlHost.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MlHost.Services
{
    public static class PackageDeploymentExtensions
    {
        public static PackageUpdate UpdateToFolderFromResourceId(this PackageUpdate packageUpdate, string deploymentFolder, Type type, string resourceId)
        {
            resourceId = resourceId.ToNullIfEmpty() ?? throw new ArgumentException(nameof(resourceId));

            using Stream? packageStream = Assembly.GetAssembly(type)!.GetManifestResourceStream(resourceId);
            if (packageStream == null) throw new ArgumentException($"Resource ID {resourceId} not located in assembly");

            packageUpdate.UpdateToFolder(deploymentFolder, packageStream);
        }
    }
}
