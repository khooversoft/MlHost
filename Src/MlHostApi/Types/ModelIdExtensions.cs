using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostApi.Types
{
    public static class ModelIdExtensions
    {
        public static string ToRegexPattern(this ModelId modelId) => $"^{modelId.Root}\\/{modelId.ModelName}\\/{modelId.VersionId}$";
    }
}
