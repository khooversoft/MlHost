using Microsoft.Extensions.Configuration;
using Toolbox.Tools;
using Toolbox.Application;
using System.Reflection;
using System.Linq;
using System;

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

        public static string HostVersionTitle(this IOption option) => $"Machine Learning Model Host - Version {option.HostVersion()}";

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
