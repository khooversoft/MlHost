using Toolbox.Tools;

namespace MlHostSdk.Types
{
    public static class ModelIdExtensions
    {
        public static string ToPath(this ModelId modelId) =>
            modelId.VerifyNotNull(nameof(modelId))
            .Func(x => $"{x}{MlPackageFile.Extension}");
    }
}
