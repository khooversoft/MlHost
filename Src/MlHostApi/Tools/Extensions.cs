using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostApi.Tools
{
    public static class Extensions
    {
        public static string? ToNullIfEmpty(this string? subject) => string.IsNullOrWhiteSpace(subject) ? null : subject;

        public static T VerifyNotNull<T>(this T? subject, string message) where T : class => subject ?? throw new ArgumentNullException(message);

        public static string VerifyNotEmpty(this string? subject, string message) => subject.ToNullIfEmpty() ?? throw new ArgumentException(message);

        public static T VerifyAssert<T>(this T subject, Func<T, bool> test, string message) => test(subject) switch { true => subject, _ => throw new ArgumentException(message) };

        public static TResult Do<T, TResult>(this T subject, Func<T, TResult> function) => function.VerifyNotNull(nameof(subject))(subject);

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
    }
}
