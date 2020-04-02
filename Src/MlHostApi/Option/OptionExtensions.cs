using Toolbox.Repository;
using Toolbox.Tools;

namespace MlHostApi.Option
{
    public static class OptionExtensions
    {
        public static void Verify(this BlobStoreOption blobStoreOption)
        {
            blobStoreOption.VerifyNotNull("BlobStore is required");
            blobStoreOption.ContainerName.VerifyNotEmpty($"{nameof(blobStoreOption.ContainerName)} is missing");
            blobStoreOption.AccountName.VerifyNotEmpty($"{nameof(blobStoreOption.AccountName)} is missing");
            blobStoreOption.AccountKey.VerifyNotEmpty($"{nameof(blobStoreOption.AccountKey)} is missing");
        }

        public static void Verify(this KeyVaultOption keyVaultOption)
        {
            keyVaultOption.VerifyNotNull("KeyVault option is required");
            keyVaultOption.KeyVaultName.VerifyNotEmpty($"{nameof(keyVaultOption.KeyVaultName)} is missing");
            keyVaultOption.KeyName.VerifyNotEmpty($"{nameof(keyVaultOption.KeyName)} is missing");
        }

        public static string CreateBlobConnectionString(this BlobStoreOption blobStoreOption) =>
            blobStoreOption.VerifyNotNull(nameof(blobStoreOption))
            .Func(x => $"DefaultEndpointsProtocol=https;AccountName={x.AccountName};AccountKey={x.AccountKey};EndpointSuffix=core.windows.net");
    }
}
