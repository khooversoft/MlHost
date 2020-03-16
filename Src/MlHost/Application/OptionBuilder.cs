using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Application
{
    public class OptionBuilder
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
            IConfigurationBuilder builder = new ConfigurationBuilder();

            string currentDirectory = Directory.GetCurrentDirectory();
            builder.SetBasePath(currentDirectory);

            if (Args != null) builder.AddCommandLine(Args);
            if (!string.IsNullOrWhiteSpace(JsonFile)) builder.AddJsonFile(JsonFile);

            IConfiguration configuration = builder.Build();

            var option = new Option();
            configuration.Bind(option, x => x.BindNonPublicProperties = true);

            option.Verify();

            return option;
        }
    }
}
