using Toolbox.Tools;

namespace MlHostApi.Types
{
    public static class ModelIdExtensions
    {
        public const string ModelPackageExtension = "mlPackage";

        public static string ToRegexPattern(this ModelId modelId) => $"^{modelId.Root}\\/{modelId.ModelName}\\/{modelId.VersionId}.{ModelPackageExtension}$";

        public static string ToBlobPath(this ModelId modelId) =>
            modelId.VerifyNotNull(nameof(modelId))
            .Func(x => $"{x}.{ModelPackageExtension}");
    }
}
