﻿using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace MlHostCli.Test.Application
{
    internal static class Function
    {
        public static string WriteResourceToFile(string fileName, string resourceId) =>
                Path.GetTempPath()
                    .Do(x => Path.Combine(x, fileName))
                    .Do(x => { WriteFile(x, GetResourceStream(resourceId)); return x; });

        public static Stream GetResourceStream(string streamId) =>
                Assembly.GetAssembly(typeof(ActivityTests))!
                    .GetManifestResourceStream(streamId)
                    .VerifyNotNull($"Cannot find {streamId} in assembly's resource");

        public static void WriteFile(string file, Stream stream)
        {
            using (stream)
            {
                using Stream writeFile = new FileStream(file, FileMode.Create);
                stream.CopyTo(writeFile);
            }
        }

        public static byte[] GetFileHash(string file)
        {
            using Stream read = new FileStream(file, FileMode.Open);
            return MD5.Create().ComputeHash(read);
        }
    }
}
