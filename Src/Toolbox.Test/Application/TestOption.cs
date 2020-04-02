using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading;
using Toolbox.Repository;
using Toolbox.Tools;

namespace Toolbox.Test.Application
{
    internal class TestOption
    {
        internal const string _resourceId = "Toolbox.Test.Application.TestConfig.json";

        private static IDatalakeRepository? _datalakeRepository;

        public static IDatalakeRepository GetDatalakeRepository()
        {
            Interlocked.CompareExchange(ref _datalakeRepository, get(), null);
            return _datalakeRepository;

            DatalakeRepository get() => GetBlobStoreOption()
                .Action(x => x.Verify())
                .Func(x => new DatalakeRepository(x));
        }

        private static StoreOption GetBlobStoreOption()
        {
            using Stream configStream = FileTools.GetResourceStream(typeof(TestOption), _resourceId);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonStream(configStream)
                .AddUserSecrets("Toolbox.Test")
                .Build();

            var option = new StoreOption();
            configuration.Bind(option, x => x.BindNonPublicProperties = true);
            option.Verify();

            return option;
        }
    }
}
