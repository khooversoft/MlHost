using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Toolbox.Models;
using Toolbox.Services;
using Toolbox.Tools;

namespace MlProcess.Application
{
    internal class OptionBuilder
    {
        public OptionBuilder() { }

        public string[]? Args { get; set; }

        public string? ConfigFile { get; set; }

        public OptionBuilder AddCommandLine(params string[] args)
        {
            Args = args.ToArray();
            return this;
        }

        public IOption Build()
        {
            if (Args == null || Args.Length == 0) return new Option { Help = true };

            // Look for switches in the model
            string[] switchNames = typeof(Option).GetProperties()
                .Where(x => x.PropertyType == typeof(bool))
                .Select(x => x.Name)
                .ToArray();

            // Add "=true" for all switches that don't have this already
            string[] args = (Args ?? Array.Empty<string>())
                .Select(x => switchNames.Contains(x, StringComparer.OrdinalIgnoreCase) ? x + "=true" : x)
                .ToArray();

            string? configFile = null;
            Option option = null!;

            // Because ordering or placement on critical configuration @can different, loop through a process
            // of building the correct configuration.  Pattern cases below are in priority order.
            while (true)
            {
                option = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddCommandLine(args)
                    .Func(x => configFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(configFile), _ => x })
                    .Build()
                    .Bind();

                switch (option)
                {
                    case Option v when v.Help:
                        return new Option { Help = true };

                    case Option v when v.ConfigFile.ToNullIfEmpty() != null && configFile == null:
                        configFile = v.ConfigFile;
                        continue;

                    case Option v when v.Run && v.Output.ToNullIfEmpty() == null:
                        v.Output = Path.ChangeExtension(v.Input, ".processed");
                        break;
                }

                break;
            };

            if (option.LogFile.ToNullIfEmpty() != null && Path.GetExtension(option.LogFile).ToNullIfEmpty() == null)
            {
                option.LogFile = Path.ChangeExtension(option.LogFile, ".log");
            }

            option.Verify();

            return option;
        }
    }
}
