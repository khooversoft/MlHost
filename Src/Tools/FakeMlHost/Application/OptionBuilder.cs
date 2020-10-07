using FakeMlHost.Application;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace FakeModelServer.Application
{
    public class OptionBuilder
    {
        public OptionBuilder() { }

        public string[]? Args { get; set; }

        public OptionBuilder SetArgs(params string[] args) => this.Action(x => x.Args = args.ToArray());

        public IOption Build()
        {
            Option option = new ConfigurationBuilder()
                .AddCommandLine(Args ?? Array.Empty<string>())
                .Build()
                .Bind<Option>();

            option.Verify();

            return option;
        }
    }
}
