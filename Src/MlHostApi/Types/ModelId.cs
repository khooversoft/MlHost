﻿using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MlHostApi.Types
{
    public class ModelId
    {
        public ModelId(string modelName, string versionId)
        {
            ModelName = modelName.ToLower().VerifyBlobVector(nameof(modelName));
            VersionId = versionId.ToLower().VerifyBlobVector(nameof(versionId));
        }

        public ModelId(string root, string modelName, string versionId)
        {
            Root = root.ToLower().VerifyBlobVector(nameof(root));
            ModelName = modelName.ToLower().VerifyBlobVector(nameof(modelName));
            VersionId = versionId.ToLower().VerifyBlobVector(nameof(VersionId));
        }

        public ModelId(string path)
        {
            path.VerifyNotEmpty(nameof(path));

            Stack<string> pathVectors = path.ToLower().Split("/", StringSplitOptions.RemoveEmptyEntries)
                .Reverse()
                .Func(x => new Stack<string>(x));

            if (pathVectors.Count < 2 || pathVectors.Count > 3) throw new ArgumentException($"{path} does not have 2 or 3 vectors");

            if (pathVectors.Count == 3) Root = pathVectors.Pop().VerifyBlobVector($"{nameof(Root)} is invalid"); ;
            ModelName = pathVectors.Pop().VerifyBlobVector($"{nameof(ModelName)} is invalid");
            VersionId = pathVectors.Pop().VerifyBlobVector($"{nameof(VersionId)} is invalid");
        }

        public static string RootName { get; } = "ml-models";

        public string Root { get; } = RootName;

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

        public static bool operator ==(ModelId? left, ModelId? right) => EqualityComparer<ModelId>.Default.Equals(left, right);

        public static bool operator !=(ModelId? left, ModelId? right) => !(left == right);
    }
}