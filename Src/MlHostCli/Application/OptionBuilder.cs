using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MlHostApi.Tools;
using System.IO;

namespace MlHostCli.Application
{
    internal class OptionBuilder
    {
        public OptionBuilder() { }

        public string[]? Args { get; set; }

        public string? ConfigFile { get; set; }

        public string? SecretId { get; set; }

        public OptionBuilder AddCommandLine(params string[] args)
        {
            Args = args.ToArray();
            return this;
        }

        public OptionBuilder AddUserSecrets(string secretId)
        {
            SecretId = secretId;
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

            // Look for "ConfigFile={file}", if specified, to process
            string? configFile = ConfigFile ?? args
                .Select(x => x.Split('=', StringSplitOptions.RemoveEmptyEntries))
                .Where(x => x.Length == 2 && x.First().Equals("ConfigFile", StringComparison.OrdinalIgnoreCase))
                .Select(x => x[1])
                .FirstOrDefault();

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Do(x => configFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(v), _ => x })
                .Do(x => SecretId.ToNullIfEmpty() switch { string v => x.AddUserSecrets(v), _ => x })
                .AddCommandLine(args)
                .Build();

            var option = new Option();
            configuration.Bind(option, x => x.BindNonPublicProperties = true);

            option.Verify();

            return option;
        }
    }
}
