﻿using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;

namespace Toolbox.Tools
{
    public static class FileTools
    {
        public static string WriteResourceToTempFile(string fileName, string folder, Type type, string resourceId)
        {
            @fileName.VerifyNotEmpty(nameof(fileName));
            folder.VerifyNotEmpty(nameof(folder));

            string filePath = Path.Combine(Path.GetTempPath(), folder, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using var stream = GetResourceStream(type, resourceId);
            WriteStreamToFile(stream, filePath);

            return filePath;
        }

        public static byte[] GetFileHash(string file)
        {
            using Stream read = new FileStream(file, FileMode.Open);
            return MD5.Create().ComputeHash(read);
        }

        /// <summary>
        /// Get stream from assembly's resources
        /// </summary>
        /// <param name="type">type int the assembly that has the resource</param>
        /// <param name="streamId">resource id</param>
        /// <returns>stream</returns>
        public static Stream GetResourceStream(this Type type, string streamId) =>
                Assembly.GetAssembly(type.VerifyNotNull(nameof(type)))!
                    .GetManifestResourceStream(streamId.VerifyNotEmpty(nameof(streamId)))
                    .VerifyNotNull($"Cannot find {streamId} in assembly's resource");

        /// <summary>
        /// Read resource and convert to string
        /// </summary>
        /// <param name="type">type int the assembly that has the resource</param>
        /// <param name="streamId">resource id</param>
        /// <returns>resource as string</returns>
        public static string GetResourceAsString(this Type type, string streamId)
        {
            using Stream stream = type.GetResourceStream(streamId);
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        public static void WriteStreamToFile(this Stream stream, string file)
        {
            using Stream writeFile = new FileStream(file, FileMode.Create);
            stream.CopyTo(writeFile);
        }

        public static async Task WriteToFile<T>(this T subject, string filePath) where T : class
        {
            subject.VerifyNotNull(nameof(subject));
            filePath.VerifyNotEmpty(nameof(filePath));

            string jsonString = Json.Default.Serialize(subject);
            await File.WriteAllTextAsync(filePath, jsonString);
        }

        public static async Task<T?> ReadFromFile<T>(string filePath) where T : class
        {
            filePath.VerifyNotEmpty(nameof(filePath));

            if (!File.Exists(filePath)) return null;

            string jsonString = await File.ReadAllTextAsync(filePath);
            return Json.Default.Deserialize<T>(jsonString);
        }

        public static void DeleteDirectory(string path)
        {
            path.VerifyNotEmpty(nameof(path));
            Directory.Delete(path, true);

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            while (Directory.Exists(path) && !timeout.Token.IsCancellationRequested)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(200));
            }
        }
    }
}
