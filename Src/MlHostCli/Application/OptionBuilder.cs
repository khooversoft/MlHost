using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MlHostApi.Tools;
using System.IO;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using MlHostApi.Option;

namespace MlHostCli.Application
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

            Option tempOption = new ConfigurationBuilder()
                    .Func(x => configFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(configFile), _ => x })
                    .AddCommandLine(args)
                    .Build()
                    .Bind();

            if (tempOption.Help)
            {
                return new Option { Help = true };
            }

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

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Func(x => configFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(v), _ => x })
                .Func(x => tempOption.SecretId.ToNullIfEmpty() switch { string v => x.AddUserSecrets(v), _ => x })
                .AddCommandLine(args
                    .Append($"{nameof(Option.BlobStore)}:{nameof(Option.BlobStore.AccountKey)}={accountKey}")
                    .ToArray())
                .Build()
                .Bind()
                .Verify();
        }
    }
}
