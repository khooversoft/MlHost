using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Toolbox.Tools;
using Toolbox.Application;

namespace MlHost.Application
{
    internal class OptionBuilder
    {
        public OptionBuilder() { }

        public string? JsonFile { get; set; }

        public string[]? Args { get; set; }

        public OptionBuilder AddCommandLine(params string[] args)
        {
            Args = args.ToArray();
            return this;
        }

        public OptionBuilder AddJsonFile(string jsonFile)
        {
            JsonFile = jsonFile;
            return this;
        }

        public IOption Build()
        {
            Option option = new ConfigurationBuilder()
                .Func(x => JsonFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(JsonFile), _ => x })
                .AddCommandLine(Args ?? Array.Empty<string>())
                .Build()
                .Bind<Option>();

            option.Verify();
            option.RunEnvironment = option.Environment.ConvertToEnvironment();

            return option;
        }
    }
}
