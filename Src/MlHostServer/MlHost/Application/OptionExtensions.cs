using Microsoft.Extensions.Configuration;
using Toolbox.Tools;
using Toolbox.Application;
using System.Reflection;

namespace MlHost.Application
{
    internal static class OptionExtensions
    {
        public static void Verify(this Option option)
        {
            option.VerifyNotNull(nameof(option));

            option.ServiceUri.VerifyNotEmpty($"{nameof(option.ServiceUri)} is missing");
            option.Environment.VerifyNotEmpty($"{nameof(option.Environment)} is missing");
            option.Environment.ConvertToEnvironment().VerifyAssert(x => x != RunEnvironment.Unknown, $"Invalid run environment {option.Environment}");
        }

        public static string HostVersion(this IOption _) => Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version!
            .ToString();

        public static string HostVersionTitle(this IOption _) => $"Machine Learning Model Host - Version {Assembly.GetExecutingAssembly().GetName().Version}";
    }
}
