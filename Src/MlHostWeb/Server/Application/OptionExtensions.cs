using System;
using System.Linq;
using Toolbox.Application;
using Toolbox.Tools;

namespace MlHostWeb.Server.Application
{
    internal static class OptionExtensions
    {
        private const string baseId = "MlHostWeb.Server.Configs";

        public static void Verify(this Option option)
        {
            option.VerifyNotNull(nameof(option));

            option.VerifyNotNull($"{nameof(option.Environment)} is required");
            option.VerifyNotNull($"{nameof(option.FrontEndUrl)} is required");

            option.Models.VerifyNotNull($"{nameof(option.Models)} is required");
            option.Models.VerifyAssert(x => x.Count > 0, $"{nameof(option.Models)} is required");
        }

        public static string ConvertToResourceId(this RunEnvironment subject) => subject switch
        {
            RunEnvironment.Dev => $"{baseId}.dev-config.json",
            RunEnvironment.Acpt => $"{baseId}.acpt-config.json",
            RunEnvironment.Prod => $"{baseId}.prod-config.json",

            _ => throw new InvalidOperationException(),
        };

        public static void DumpConfigurations(this IOption option)
        {
            const int maxWidth = 80;

            option.GetConfigValues()
                .Select(x => "  " + x)
                .Prepend(new string('=', maxWidth))
                .Prepend("Current configurations")
                .Prepend(string.Empty)
                .Append(string.Empty)
                .Append(string.Empty)
                .ForEach(x => Console.WriteLine(x));
        }
    }
}
