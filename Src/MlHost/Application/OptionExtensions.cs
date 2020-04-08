using Microsoft.Extensions.Configuration;
using Toolbox.Tools;

namespace MlHost.Application
{
    internal static class OptionExtensions
    {
        public static void Verify(this Option option)
        {
            option.VerifyNotNull(nameof(option));

            option.ServiceUri.VerifyNotEmpty($"{nameof(option.ServiceUri)} is missing");
            option.DeploymentFolder.VerifyNotEmpty($"{nameof(option.DeploymentFolder)} is missing");
        }

        public static Option Bind(this IConfiguration configuration)
        {
            configuration.VerifyNotNull(nameof(configuration));

            var option = new Option();
            configuration.Bind(option, x => x.BindNonPublicProperties = true);
            return option;
        }
    }
}
