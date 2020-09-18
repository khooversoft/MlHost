using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Tools
{
    public static class Extensions
    {
        [DebuggerStepThrough]
        public static string? ToNullIfEmpty(this string? subject) => string.IsNullOrWhiteSpace(subject) ? null : subject;

        [DebuggerStepThrough]
        public static T VerifyNotNull<T>(this T? subject, string message) where T : class => subject ?? throw new ArgumentNullException(message);

        [DebuggerStepThrough]
        public static string VerifyNotEmpty(this string? subject, string message) => subject.ToNullIfEmpty() ?? throw new ArgumentException(message);

        [DebuggerStepThrough]
        public static T VerifyAssert<T>(this T subject, Func<T, bool> test, string message) => test(subject) switch
        {
            true => subject,
            _ => throw new ArgumentException(message)
        };

        [DebuggerStepThrough]
        public static T VerifyAssert<T, TException>(this T subject, Func<T, bool> test, Func<T, string> message) where TException : Exception => test(subject) switch
        {
            true => subject,
            _ => throw (Exception)Activator.CreateInstance(typeof(T), message(subject))!
        };

        public static TResult Func<T, TResult>(this T subject, Func<T, TResult> function) => function.VerifyNotNull(nameof(subject))(subject);

        public static T Action<T>(this T subject, Action<T> action)
        {
            action.VerifyNotNull(nameof(action));

            action.VerifyNotNull(nameof(subject))(subject);
            return subject;
        }

        public static void ForEach<T>(this IEnumerable<T> subjects, Action<T> action)
        {
            subjects.VerifyNotNull(nameof(subjects));
            action.VerifyNotNull(nameof(action));

            foreach (var item in subjects)
            {
                action(item);
            }
        }

        public static async Task ForEachAsync<T>(this IEnumerable<T> subjects, Func<T, Task> action)
        {
            subjects.VerifyNotNull(nameof(subjects));
            action.VerifyNotNull(nameof(action));

            foreach (var item in subjects)
            {
                await action(item);
            }
        }

        public static SubjectScope<T> ToSubjectScope<T>(this T subject) where T : class => new SubjectScope<T>(subject.VerifyNotNull(nameof(subject)));

        public static T Bind<T>(this IConfiguration configuration) where T : new()
        {
            configuration.VerifyNotNull(nameof(configuration));

            var option = new T();
            configuration.Bind(option, x => x.BindNonPublicProperties = true);
            return option;
        }
    }
}
