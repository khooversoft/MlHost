using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using System.Threading;
using Toolbox.Models;
using Toolbox.Repository;
using Toolbox.Tools;

namespace Toolbox.Test.Application
{
    internal class TestOption
    {
        internal const string _resourceId = "Toolbox.Test.Application.TestConfig.json";

        private static IDatalakeStore? _datalakeRepository;

        public static IDatalakeStore GetDatalakeRepository()
        {
            Interlocked.CompareExchange(ref _datalakeRepository, get(), null);
            return _datalakeRepository;

            DatalakeStore get() => GetBlobStoreOption()
                .Func(x => new DatalakeStore(x, new NullLogger<DatalakeStore>()));
        }

        private static StoreOption GetBlobStoreOption()
        {
            using Stream configStream = FileTools.GetResourceStream(typeof(TestOption), _resourceId);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonStream(configStream)
                .AddUserSecrets("Toolbox.Test")
                .Build();

            var option = new StoreOption()
                .Action(x => configuration.Bind(x, x => x.BindNonPublicProperties = true))
                .Action(x => x.Verify());

            return option;
        }
    }
}
