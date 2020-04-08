using Toolbox.Tools;

namespace MlHostApi.Types
{
    public static class ModelIdExtensions
    {
        public static string ToPath(this ModelId modelId) =>
            modelId.VerifyNotNull(nameof(modelId))
            .Func(x => $"{x}{MlPackageFile.Extension}");
    }
}
