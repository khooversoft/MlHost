using Microsoft.Extensions.Configuration;
using MlHost.Tools;
using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MlHost.Application
{
    internal class OptionBuilder
    {
        public OptionBuilder() { }

        public string? JsonFile { get; set; }

        public string? SecretId { get; set; }

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

        public OptionBuilder AddUserSecrets(string secretId)
        {
            SecretId = secretId;
            return this;
        }

        public IOption Build()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Func(x => JsonFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(v), _ => x })
                .Func(x => SecretId.ToNullIfEmpty() switch { string v => x.AddUserSecrets(v), _ => x })
                .AddCommandLine(Args ?? Array.Empty<string>())
                .Build();

            var option = new Option();
            configuration.Bind(option, x => x.BindNonPublicProperties = true);

            option.Verify();

            option.Deployment!.DeploymentFolder = BuildPathRelativeFromExceutingAssembly(option.Deployment.DeploymentFolder);
            option.Deployment!.PackageFolder = BuildPathRelativeFromExceutingAssembly(option.Deployment.PackageFolder);

            return option;
        }

        public static string BuildPathRelativeFromExceutingAssembly(string folder)
        {
            if (Path.GetDirectoryName(folder).ToNullIfEmpty() != null) return folder;

            return Assembly.GetExecutingAssembly()
                .Func(x => Path.GetDirectoryName(x.Location))
                .Func(x => Path.Combine(x!, folder));
        }
    }
}
