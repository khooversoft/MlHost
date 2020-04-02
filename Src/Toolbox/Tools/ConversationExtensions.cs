using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Toolbox.Tools
{
    public static class ConversationExtensions
    {
        public static IReadOnlyDictionary<string, object>? ToDictionary(this object subject) =>
            subject
            .Func(x => x.ToExpandoObject())
            .Func(x => x as IDictionary<string, object>)
            .Func(x => x?.ToDictionary(x => x.Key, x => x.Value));

        public static ExpandoObject ToExpandoObject(this object subject)
        {
            subject.VerifyNotNull(nameof(subject));

            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor? property in TypeDescriptor.GetProperties(subject.GetType()))
            {
                expando.Add(property!.Name, property.GetValue(subject));
            }

            return (ExpandoObject)expando;
        }

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

        public static byte[] ToMd5Hash(this byte[] subject) => MD5.Create().ComputeHash(subject);

        public static byte[] ToBytes(this string subject) => Encoding.UTF8.GetBytes(subject);
    }
}
