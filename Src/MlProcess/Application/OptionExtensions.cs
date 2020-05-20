using Microsoft.Extensions.Configuration;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Toolbox.Tools;

namespace MlProcess.Application
{
    internal static class OptionExtensions
    {
        private static readonly Func<ModelOption, bool> _verifyModel = x =>
            x.Uri.VerifyNotEmpty($"{nameof(x.Uri)} is required in all models") != null
            && x.Name.VerifyNotEmpty($"{nameof(x.Name)} is required in all models") != null;

        private static readonly IReadOnlyList<Func<Option, bool>> _scenarios = new List<Func<Option, bool>>
        {
            // Scenarios are listed in priority order
            option => option.Run
                && option.Input.VerifyNotEmpty($"{nameof(option.Input)} file is required for {nameof(option.Run)}") != null
                && option.Input.VerifyAssert(x => File.Exists(x), $"{nameof(option.Input)} file does not exist") != null
                && option.Output.VerifyNotEmpty($"{nameof(option.Output)} file is required for {nameof(option.Run)}") != null
                && option.Models.VerifyAssert(x => x != null && x.Count > 0, "Models must be specified") != null
                && option.Models.All(x => _verifyModel(x)),

            option => option.Append
                && option.Input.VerifyNotEmpty($"{nameof(option.Input)} file is required for {nameof(option.Run)}") != null
                && option.Output.VerifyNotEmpty($"{nameof(option.Output)} file is required for {nameof(option.Run)}") != null
                && option.Models.VerifyAssert(x => x != null && x.Count > 0, "Models must be specified") != null
                && option.Models.All(x => _verifyModel(x)),
        };

        public static Option Verify(this Option option)
        {
            _scenarios
                .Select(x => x(option) ? 1 : 0)
                .Sum()
                .VerifyAssert(x => x != 0, $"Unknown command(s).  Use 'help' to list valid commands");

            return option;
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
