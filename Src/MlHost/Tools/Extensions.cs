using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Tools
{
    internal static class Extensions
    {
        public static string? ToNullIfEmpty(this string subject) => string.IsNullOrWhiteSpace(subject) ? null : subject;
    }
}
