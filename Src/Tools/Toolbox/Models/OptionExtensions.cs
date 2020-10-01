﻿using Toolbox.Tools;

namespace Toolbox.Models
{
    public static class OptionExtensions
    {
        public static void Verify(this KeyVaultOption keyVaultOption)
        {
            keyVaultOption.VerifyNotNull("KeyVault option is required");
            keyVaultOption.KeyVaultName.VerifyNotEmpty($"{nameof(keyVaultOption.KeyVaultName)} is missing");
            keyVaultOption.KeyName.VerifyNotEmpty($"{nameof(keyVaultOption.KeyName)} is missing");
        }
    }
}