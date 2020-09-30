using System;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHost.Tools
{
    internal class Activity
    {
        private readonly Func<Task> _func;

        public Activity(string description, Func<Task> func)
        {
            description.VerifyNotEmpty(nameof(description));
            func.VerifyNotNull(nameof(func));

            Description = description;
            _func = func;
        }

        public string Description { get; }

        public Task Execute() => _func();
    }
}
