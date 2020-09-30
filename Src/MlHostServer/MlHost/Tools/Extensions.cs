using System;

namespace MlHost.Tools
{
    internal static class Extensions
    {
        public static T Resolve<T>(this IServiceProvider serviceProvider) where T : class => (T)serviceProvider.GetService(typeof(T)) ?? throw new ArgumentException("Not found in service provider");
    }
}
