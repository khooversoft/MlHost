using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHost.Tools
{
    internal class Activity
    {
        private readonly Func<Task<bool>> _func;

        public Activity(string description, Func<Task<bool>> func)
        {
            description.VerifyNotEmpty(nameof(description));
            func.VerifyNotNull(nameof(func));

            Description = description;
            _func = func;
        }

        public string Description { get; }

        public Task<bool> Execute() => _func();
    }
}
