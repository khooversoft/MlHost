using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading;
using Toolbox.Repository;
using Toolbox.Tools;

namespace Toolbox.Test.Application
{
    internal class TestOption
    {
        internal static readonly string _resourceId = "Toolbox.Test.Application.TestConfig.json";

        private static IBlobRepository? _blobRepository;
        private static IDatalakeRepository? _datalakeRepository;

        public static IBlobRepository GetBlobRepository()
        {
            Interlocked.CompareExchange(ref _blobRepository, get(), null);
            return _blobRepository;

            BlobRepository get() => GetBlobStoreOption()
                .BlobStore
                .VerifyNotNull("BlobStore")
                .Action(x => x.Verify())
                .Func(x => new BlobRepository(x));
        }
        
        public static IBlobRepository GetBlobRepositoryForDataLake()
        {
            Interlocked.CompareExchange(ref _blobRepository, get(), null);
            return _blobRepository;

            BlobRepository get() => GetBlobStoreOption()
                .DatalakeStore
                .VerifyNotNull("DatalakeStore")
                .Action(x => x.Verify())
                .Func(x => new BlobRepository(x));
        }

        public static IDatalakeRepository GetDatalakeRepository()
        {
            Interlocked.CompareExchange(ref _datalakeRepository, get(), null);
            return _datalakeRepository;

            DatalakeRepository get() => GetBlobStoreOption()
                .DatalakeStore
                .VerifyNotNull("DatalakeStore")
                .Action(x => x.Verify())
                .Func(x => new DatalakeRepository(x));
        }

        private static Option GetBlobStoreOption()
        {
            using Stream configStream = FileTools.GetResourceStream(typeof(TestOption), _resourceId);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonStream(configStream)
                .AddUserSecrets("Toolbox.Test")
                .Build();

            var option = new Option();
            configuration.Bind(option, x => x.BindNonPublicProperties = true);

            return option;
        }

        private class Option
        {
            public BlobStoreOption? BlobStore { get; set; }

            public BlobStoreOption? DatalakeStore { get; set; }
        }
    }
}
