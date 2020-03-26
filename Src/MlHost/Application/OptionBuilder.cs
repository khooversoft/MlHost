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

            string? secretId = SecretId;
            string? accountKey = null;
            Option option;

            Func<string, string> createAccountKeyCommand = x => $"{nameof(option.BlobStore)}:{nameof(option.BlobStore.AccountKey)}=" + x;

            while (true)
            {
                option = new ConfigurationBuilder()
                    .Func(x => JsonFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(JsonFile), _ => x })
                    .Func(x => (secretId.ToNullIfEmpty()) switch { string v => x.AddUserSecrets(v), _ => x })
                    .AddCommandLine(args.Concat(accountKey switch { string v => new[] { createAccountKeyCommand(accountKey) }, _ => Enumerable.Empty<string>() }).ToArray())
                    .Build()
                    .Bind();

                switch (option)
                {
                    case Option v when v.SecretId.ToNullIfEmpty() != null && secretId == null:
                        secretId = v.SecretId;
                        continue;

                    case Option v when v.BlobStore?.AccountKey == null && accountKey == null:
                        v.KeyVault!.Verify();

                        var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback));

                        accountKey = new ConfigurationBuilder()
                            .AddAzureKeyVault($"https://{option.KeyVault!.KeyVaultName}.vault.azure.net/", keyVaultClient, new DefaultKeyVaultSecretManager())
                            .Build()[option.KeyVault!.KeyName];

                        if (accountKey != null) continue;
                        break;
                }

                break;
            }

            option.Verify();

            option.Deployment.DeploymentFolder = BuildPathRelativeFromExceutingAssembly(option.Deployment.DeploymentFolder);
            option.Deployment.PackageFolder = BuildPathRelativeFromExceutingAssembly(option.Deployment.PackageFolder);

            return option;
        }

        private static string BuildPathRelativeFromExceutingAssembly(string folder)
        {
            if (Path.GetDirectoryName(folder).ToNullIfEmpty() != null) return folder;

            return Assembly.GetExecutingAssembly()
                .Func(x => Path.GetDirectoryName(x.Location))
                .Func(x => Path.Combine(x!, folder));
        }
    }
}
