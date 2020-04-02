using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Toolbox.Tools
{
    public static class FileTools
    {
        public static string WriteResourceToTempFile(string fileName, Type type, string resourceId) =>
                Path.GetTempPath()
                    .Func(path => Path.Combine(path, Guid.NewGuid().ToString(), fileName))
                    .Action(path => Directory.CreateDirectory(Path.GetDirectoryName(path)))
                    .Action(file => WriteStreamToFile(GetResourceStream(type, resourceId), file));

        public static Stream GetResourceStream(Type type, string streamId) =>
                Assembly.GetAssembly(type.VerifyNotNull(nameof(type)))!
                    .GetManifestResourceStream(streamId.VerifyNotEmpty(nameof(streamId)))
                    .VerifyNotNull($"Cannot find {streamId} in assembly's resource");

        public static void WriteStreamToFile(Stream stream, string file)
        {
            using Stream writeFile = new FileStream(file, FileMode.Create);
            stream.CopyTo(writeFile);
        }

        public static byte[] GetFileHash(string file)
        {
            using Stream read = new FileStream(file, FileMode.Open);
            return MD5.Create().ComputeHash(read);
        }
    }
}
