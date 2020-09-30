using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Tools;
using Toolbox.Application;
using System.Reflection;

namespace MlFrontEnd.Application
{
    internal class OptionBuilder
    {
        public OptionBuilder() { }

        public string[]? Args { get; set; }

        public string? ConfigFile { get; set; }

        public OptionBuilder SetArgs(params string[] args)
        {
            Args = args.ToArray();
            return this;
        }

        public OptionBuilder SetConfigFile(string configFile)
        {
            ConfigFile = configFile;
            return this;
        }

        public IOption Build()
        {
            // Look for switches in the model
            string[] switchNames = typeof(Option).GetProperties()
                .Where(x => x.PropertyType == typeof(bool))
                .Select(x => x.Name)
                .ToArray();

            // Add "=true" for all switches that don't have this already
            string[] args = (Args ?? Array.Empty<string>())
                .Select(x => switchNames.Contains(x, StringComparer.OrdinalIgnoreCase) ? x + "=true" : x)
                .ToArray();

            string? secretId = null;
            Option option = new Option();

            // Because ordering or placement on critical configuration can different, loop through a process
            // of building the correct configuration.  Pattern cases below are in priority order.
            while (true)
            {
                option = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddEnvironmentVariables("MlFrontEnd_")
                    .Func(x => GetEnvironmentConfig(option) switch { Stream v => x.AddJsonStream(v), _ => x })
                    .Func(x => secretId.ToNullIfEmpty() switch { string v => x.AddUserSecrets(v), _ => x })
                    .AddCommandLine(args)
                    .Build()
                    .Bind<Option>();

                switch (option)
                {
                    case Option v when v.SecretId.ToNullIfEmpty() != null && secretId == null:
                        secretId = v.SecretId;
                        continue;
                }

                break;
            };

            option.Verify();
            option.RunEnvironment = option.Environment.ConvertToEnvironment();

            return option;
        }

        private Stream? GetEnvironmentConfig(Option option)
        {
            if (option?.Environment?.ToNullIfEmpty() == null) return null;

            string resourceId = option.Environment
                .ConvertToEnvironment()
                .ConvertToResourceId();

            return Assembly.GetAssembly(typeof(OptionBuilder))
                !.GetManifestResourceStream(resourceId)
                .VerifyNotNull($"{resourceId} not found");
        }
    }
}
