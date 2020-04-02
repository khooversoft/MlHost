using Toolbox.Repository;
using Toolbox.Tools;

namespace MlHostApi.Option
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
