using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MlHostApi.Types
{
    public class ModelId
    {
        public ModelId(string modelName, string versionId)
        {
            ModelName = modelName.VerifyBlobVector(nameof(modelName));
            VersionId = versionId.VerifyBlobVector(nameof(VersionId));
        }

        public ModelId(string path)
        {
            path.VerifyNotEmpty(nameof(path));

            string[] pathVectors = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (pathVectors.Length != 2) throw new ArgumentException($"{path} is invalid");

            int index = 0;
            ModelName = pathVectors[index++].VerifyBlobVector($"{nameof(ModelName)} is invalid");
            VersionId = pathVectors[index++].VerifyBlobVector($"{nameof(VersionId)} is invalid");
        }

        public string Root { get; } = "ml-models";

        public string ModelName { get; }

        public string VersionId { get; }

        public override string ToString() => Root + "/" + ModelName + "/" + VersionId;

        public override bool Equals(object? obj)
        {
            return obj is ModelId id &&
                   Root == id.Root &&
                   ModelName == id.ModelName &&
                   VersionId == id.VersionId;
        }

        public override int GetHashCode() => HashCode.Combine(Root, ModelName, VersionId);

        public static implicit operator string(ModelId modelBlobPath) => modelBlobPath.ToString();

        public static bool operator ==(ModelId left, ModelId right) => EqualityComparer<ModelId>.Default.Equals(left, right);

        public static bool operator !=(ModelId left, ModelId right) => !(left == right);
    }
}
