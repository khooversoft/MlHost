using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Application;
using Toolbox.Tools;

namespace MlFrontEnd.Application
{
    internal static class OptionExtensions
    {
        private const string baseId = "MlFrontEnd.Configs";

        public static void Verify(this IOption option)
        {
            option.VerifyNotNull(nameof(option));

            option.Environment
                .VerifyNotEmpty("Environment is required");

            option.Hosts
                .VerifyNotNull("Host is required")
                .VerifyAssert(x => x.Count > 0, "There must be at least 1 host specified")
                .Select(x => x.ModelName!.ToLower())
                .GroupBy(x => x)
                .ForEach(x => x.Count().VerifyAssert(x => x == 1, "Duplicate versionIds are not allowed"));

            option.Hosts!
                .ForEach(x => x.Verify());
        }

        public static string? ConvertToResourceId(this RunEnvironment subject) => subject switch
        {
            RunEnvironment.Dev => $"{baseId}.dev-config.json",
            RunEnvironment.Acpt => $"{baseId}.acpt-config.json",
            RunEnvironment.Prod => $"{baseId}.prod-config.json",

            _ => null,
        };

        public static void Verify(this HostOption hostOption)
        {
            hostOption.VerifyNotNull("Host is required");
            hostOption.ModelName.VerifyNotEmpty("VersionId is required");
            hostOption.Uri.VerifyNotEmpty($"Uri for {hostOption.ModelName} is required");
        }

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
