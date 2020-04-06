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
            type.VerifyNotNull(nameof(type));
            resourceId.VerifyNotNull(nameof(resourceId));

            _type = type;
            _resourceId = resourceId;
        }

        public Task<bool> GetPackageIfRequired(bool overwrite) => throw new NotImplementedException();

        public Task<Stream> GetStream() => Task.FromResult(_type.GetResourceStream(_resourceId));
    }
}
