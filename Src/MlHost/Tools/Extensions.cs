using Microsoft.Extensions.DependencyInjection;
using MlHostApi.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Tools
{
    internal static class Extensions
    {
        public static T Resolve<T>(this IServiceProvider serviceProvider) where T : class => (T)serviceProvider.GetService(typeof(T)) ?? throw new ArgumentException("Not found in service provider");

        public static IReadOnlyDictionary<string, object>? ToDictionary(this object subject) =>
            subject
            .Func(x => x.ToExpandoObject())
            .Func(x => x as IDictionary<string, object>)
            .Func(x => x != null ? x.ToDictionary(x => x.Key, x => x.Value) : null);

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
    }
}
