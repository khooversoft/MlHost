using Microsoft.Extensions.Options;
using Toolbox.Tools;

namespace Toolbox.Models
{
    public static class OptionExtensions
    {
        public static void Verify(this KeyVaultOption? keyVaultOption)
        {
            keyVaultOption.VerifyNotNull("KeyVault option is required");
            keyVaultOption!.KeyVaultName.VerifyNotEmpty($"{nameof(keyVaultOption.KeyVaultName)} is missing");
            keyVaultOption.KeyName.VerifyNotEmpty($"{nameof(keyVaultOption.KeyName)} is missing");
        }

        public static void Verify(this StoreOption? storeOption)
        {
            storeOption.VerifyNotNull("StoreOption is required");

            storeOption!.ContainerName.VerifyNotEmpty($"{storeOption.ContainerName} is required");
            storeOption.AccountName.VerifyNotEmpty($"{storeOption.AccountName} is required");
            storeOption.AccountKey.VerifyNotEmpty($"{storeOption.AccountKey} is required");
        }
    }
}
