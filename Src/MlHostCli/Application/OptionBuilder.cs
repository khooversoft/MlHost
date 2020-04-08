﻿using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using System;
using System.IO;
using System.Linq;
using Toolbox.Models;
using Toolbox.Services;
using Toolbox.Tools;

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
            if (Args == null || Args.Length == 0) return new Option { Help = true };

            // Look for switches in the model
            string[] switchNames = typeof(Option).GetProperties()
                .Where(x => x.PropertyType == typeof(bool))
                .Select(x => x.Name)
                .ToArray();

            // Add "=true" for all switches that don't have this already
            string[] args = (Args ?? Array.Empty<string>())
                .Select(x => switchNames.Contains(x, StringComparer.OrdinalIgnoreCase) ? x + "=true" : x)
                .ToArray();

            string? configFile = null;
            string? secretId = null;
            string? accountKey = null;
            Option option = null!;

            Func<string, string> createAccountKeyCommand = x => $"{nameof(option.Store)}:{nameof(option.Store.AccountKey)}=" + x;

            // Because ordering or placement on critical configuration can different, loop through a process
            // of building the correct configuration.  Pattern cases below are in priority order.
            while (true)
            {
                option = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .Func(x => configFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(configFile), _ => x })
                    .Func(x => secretId.ToNullIfEmpty() switch { string v => x.AddUserSecrets(v), _ => x })
                    .AddCommandLine(args.Concat(accountKey switch { string v => new[] { createAccountKeyCommand(accountKey) }, _ => Enumerable.Empty<string>() }).ToArray())
                    .Build()
                    .Bind();

                switch (option)
                {
                    case Option v when v.Help:
                        return new Option { Help = true };

                    case Option v when v.ConfigFile.ToNullIfEmpty() != null && configFile == null:
                        configFile = v.ConfigFile;
                        continue;

                    case Option v when v.Store?.AccountKey == null && v.SecretId.ToNullIfEmpty() != null && secretId == null:
                        secretId = v.SecretId;
                        continue;

                    case Option v when v.Store?.AccountKey.ToNullIfEmpty() == null && accountKey == null:
                        Console.WriteLine("Getting secret from Key Vault");
                        option.KeyVault!.Verify();

                        var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback));

                        accountKey = new ConfigurationBuilder()
                            .AddAzureKeyVault($"https://{option.KeyVault!.KeyVaultName}.vault.azure.net/", keyVaultClient, new DefaultKeyVaultSecretManager())
                            .Build()[option.KeyVault!.KeyName];

                        if (accountKey != null) continue;
                        break;
                }

                break;
            };

            if (option.Store?.AccountKey.ToNullIfEmpty() != null)
            {
                option.SecretFilter = new SecretFilter(new[] { option.Store.AccountKey! });
            }

            return option.Verify();
        }
    }
}
