using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Tools
{
    internal static class Extensions
    {
        public static T Resolve<T>(this IServiceProvider serviceProvider) where T : class => (T)serviceProvider.GetService(typeof(T)) ?? throw new ArgumentException("Not found in service provider");
    }
}
