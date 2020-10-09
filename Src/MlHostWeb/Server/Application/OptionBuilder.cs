using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Toolbox.Tools;
using Toolbox.Application;
using System.Net.Sockets;

namespace MlHostWeb.Server.Application
{
    internal class OptionBuilder
    {
        public OptionBuilder() { }

        public string[]? Args { get; set; }

        public OptionBuilder SetArgs(params string[] args)
        {
            Args = args;
            return this;
        }

        public IOption Build()
        {
            Stream? stream = null;
            string? configFile = null;
            Option? option;

            while (true)
            {
                option = new ConfigurationBuilder()
                    .Func(x => configFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(configFile), _ => x })
                    .Func(x => stream switch { Stream v => x.AddJsonStream(v), _ => x })
                    .AddCommandLine(Args ?? Array.Empty<string>())
                    .Build()
                    .Bind<Option>();

                switch (option)
                {
                    case Option v when v.ConfigFile.ToNullIfEmpty() != null && configFile == null:
                        configFile = v.ConfigFile;
                        continue;

                    case Option v when v.Environment.ToNullIfEmpty() != null && stream == null:

                        string resourceId = v.Environment
                            .ConvertToEnvironment()
                            .ConvertToResourceId();

                        stream = Assembly.GetAssembly(typeof(OptionBuilder))
                            !.GetManifestResourceStream(resourceId)
                            .VerifyNotNull($"{resourceId} not found");

                        continue;
                }

                break;
            }

            option.RunEnvironment = option.Environment.ConvertToEnvironment();
            option.Verify();

            return option;
        }
    }
}
