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
                .VerifyNotEmpty("Environment is required")
                .VerifyAssert(x => x.ConvertToEnvironment() != RunEnvironment.Unknown, "Invalid environment");

            option.Hosts
                .VerifyNotNull("Host is required")
                .VerifyAssert(x => x.Count > 0, "There must be at least 1 host specified")
                .Select(x => x.VersionId!.ToLower())
                .GroupBy(x => x)
                .ForEach(x => x.Count().VerifyAssert(x => x == 1, "Duplicate versionIds are not allowed"));

            option.Hosts!
                .ForEach(x => x.Verify());
        }

        public static string ConvertToResourceId(this RunEnvironment subject) => subject switch
        {
            RunEnvironment.Dev => $"{baseId}.dev-config.json",
            RunEnvironment.Acpt => $"{baseId}.acpt-config.json",
            RunEnvironment.Prod => $"{baseId}.prod-config.json",

            _ => throw new InvalidOperationException(),
        };

        //public static HostOption GetHostOption(this IOption option, string versionId) => option.VerifyNotNull(nameof(option))
        //    .Hosts
        //    .Where(x => string.Equals(x.VersionId, versionId, StringComparison.OrdinalIgnoreCase))
        //    .FirstOrDefault();

        public static void Verify(this HostOption hostOption)
        {
            hostOption.VerifyNotNull("Host is required");
            hostOption.VersionId.VerifyNotEmpty("VersionId is required");
            hostOption.Uri.VerifyNotEmpty($"Uri for {hostOption.VersionId} is required");
        }
    }
}
