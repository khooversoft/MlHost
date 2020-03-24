using Microsoft.Extensions.Configuration;
using MlHost.Tools;
using MlHostApi.Tools;
using MlHostApi.Option;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;

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
            string[] args = Args ?? Array.Empty<string>();

            Option tempOption = new ConfigurationBuilder()
                .Func(x => JsonFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(JsonFile), _ => x })
                .AddCommandLine(args)
                .Build()
                .Bind();

            string accountKey = tempOption.BlobStore?.AccountKey switch
            {
                string v => v,

                _ => new ConfigurationBuilder()
                    .Func(x =>
                    {
                        tempOption.KeyVault!.Verify();

                        var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback));
                        x.AddAzureKeyVault($"https://{tempOption.KeyVault!.KeyVaultName}.vault.azure.net/", keyVaultClient, new DefaultKeyVaultSecretManager());
                        return x.Build()[tempOption.KeyVault!.KeyName];
                    }),
            };

            Option option = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Func(x => JsonFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(v), _ => x })
                .Func(x => (tempOption.SecretId.ToNullIfEmpty() ?? SecretId) switch { string v => x.AddUserSecrets(v), _ => x })
                .AddCommandLine(args)
                .Build()
                .Bind();

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
