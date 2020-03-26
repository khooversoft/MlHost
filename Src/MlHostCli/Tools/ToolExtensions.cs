using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MlHostCli.Tools
{
    public static class ToolExtensions
    {
        public static IReadOnlyList<string> GetConfigValues<T>(this T subject) where T : class
        {
            subject.VerifyNotNull(nameof(subject));

            static string buildName(string? root, string name) =>
                (root.ToNullIfEmpty() == null ? string.Empty : root + ":") + name;

            static bool isValue(object value) =>
                value.GetType() == typeof(string) || !value.GetType().IsClass;

            static IEnumerable<string> getProperties(string? root, object subject) =>
                subject.GetType().GetProperties()
                .SelectMany(x => getProperty(root, x.Name, x.GetValue(subject)));

            static IEnumerable<string> getProperty(string? root, string name, object? subject) => subject switch
            {
                object v when isValue(v) => new[] { $"{buildName(root, name)}={v.ToString()!}" },
                object v => getProperties(buildName(root, name), v),
                _ => Enumerable.Empty<string>(),
            };

            return getProperties(null, subject)
                .ToList();
        }
    }
}
