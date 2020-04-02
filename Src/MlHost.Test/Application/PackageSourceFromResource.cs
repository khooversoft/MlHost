using MlHost.Services;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHost.Test.Application
{
    internal class PackageSourceFromResource : IPackageSource
    {
        private readonly Type _type;
        private readonly string _resourceId;

        public PackageSourceFromResource(Type type, string resourceId)
        {
            if (type == null) throw new ArgumentException(nameof(type));
            resourceId = resourceId.ToNullIfEmpty() ?? throw new ArgumentException(nameof(resourceId));

            _type = type;
            _resourceId = resourceId;
        }

        public Task<bool> GetPackageIfRequired(bool overwrite) => throw new NotImplementedException();

        public Task<Stream> GetStream()
        {
            Stream? packageStream = Assembly.GetAssembly(_type)!.GetManifestResourceStream(_resourceId);
            if (packageStream == null) throw new ArgumentException($"Resource ID {_resourceId} not located in assembly");

            return Task.FromResult(packageStream);
        }
    }
}
