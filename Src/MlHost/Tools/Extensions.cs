﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Tools
{
    internal static class Extensions
    {
        public static string? ToNullIfEmpty(this string? subject) => string.IsNullOrWhiteSpace(subject) ? null : subject;

        public static T VerifyNotNull<T>(this T subject, string message) where T : class => subject ?? throw new ArgumentNullException(message);

        public static string VerifyNotEmpty(this string subject, string message) => subject.ToNullIfEmpty() ?? throw new ArgumentException(message);

        public static TResult Do<T, TResult>(this T subject, Func<T, TResult> function) => function.VerifyNotNull(nameof(subject))(subject);
    }
}
