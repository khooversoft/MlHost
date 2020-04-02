using Microsoft.Extensions.DependencyInjection;
using MlHostApi.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHost.Tools
{
    internal static class Extensions
    {
        public static T Resolve<T>(this IServiceProvider serviceProvider) where T : class => (T)serviceProvider.GetService(typeof(T)) ?? throw new ArgumentException("Not found in service provider");
    }
}
