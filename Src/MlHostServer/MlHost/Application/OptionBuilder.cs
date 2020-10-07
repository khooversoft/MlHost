using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Toolbox.Tools;
using Toolbox.Application;
using System.Collections.Generic;
using Toolbox.Services;

namespace MlHost.Application
{
    internal class OptionBuilder
    {
        public OptionBuilder() { }

        public string? JsonFile { get; set; }

        public string[]? Args { get; set; }

        public OptionBuilder SetArgs(params string[] args) => this.Action(x => x.Args = args.ToArray());

        public OptionBuilder SetJsonFile(string jsonFile) => this.Action(x => x.JsonFile = jsonFile);

        public IOption Build()
        {
            Option option = new ConfigurationBuilder()
                .Func(x => JsonFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(JsonFile), _ => x })
                .AddCommandLine(Args ?? Array.Empty<string>())
                .Build()
                .Bind<Option>();

            option.Verify();
            option.RunEnvironment = option.Environment.ConvertToEnvironment();

            option.PropertyResolver = new PropertyResolver(new[]
            {
                new KeyValuePair<string, string>("port", option.Port.ToString()),
                new KeyValuePair<string, string>("modelPort", option.ModelPort.ToString()),
                new KeyValuePair<string, string>("serviceUri", option.ServiceUri),
                new KeyValuePair<string, string>("runEnvironment", option.RunEnvironment.ToString()),
            });

            return option;
        }
    }
}
