using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

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

        /// <summary>
        /// Uses function to recursive build configuration settings (.Net Core Configuration) from a class that can have sub-classes
        /// </summary>
        /// <typeparam name="T">type of class</typeparam>
        /// <param name="subject">instance of class</param>
        /// <returns>list or configuration settings "propertyName=value"</returns>
        public static IReadOnlyList<string> GetConfigValues<T>(this T subject) where T : class
        {
            subject.VerifyNotNull(nameof(subject));

            return getProperties(null, subject)
                .ToList();

            static string buildName(string? root, string name) => (root.ToNullIfEmpty() == null ? string.Empty : root + ":") + name;

            static IEnumerable<string> getProperties(string? root, object subject) =>
                subject.GetType().GetProperties()
                .SelectMany(x => getProperty(root, x.Name, x.GetValue(subject)));

            static IEnumerable<string> getProperty(string? root, string name, object? subject) => subject switch
            {
                object v when v.GetType() == typeof(string) || !v.GetType().IsClass => new[] { $"{buildName(root, name)}={v!}" },
                object v => getProperties(buildName(root, name), v),
                _ => Enumerable.Empty<string>(),
            };
        }
    }
}
