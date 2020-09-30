using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Toolbox.Tools;

namespace MlHostSdk.Types
{
    public class MlPackageFile
    {
        public MlPackageFile(string filePath)
        {
            filePath.VerifyNotEmpty(nameof(filePath));

            FilePath = Path.HasExtension(filePath) ? filePath : Path.ChangeExtension(filePath, Extension);
        }

        public MlPackageFile(ModelId modelId)
        {
            modelId.VerifyNotNull(nameof(modelId));

            FilePath = modelId.ToString()
                .Replace("/", "_")
                .Func(x => x + Extension);
        }

        public MlPackageFile(ModelId modelId, string directory)
            : this(modelId)
        {
            directory.VerifyNotEmpty(nameof(directory));

            FilePath = Path.Combine(directory, FilePath);
        }

        public string FilePath { get; }

        public static string Extension = ".mlPackage";
    }
}
