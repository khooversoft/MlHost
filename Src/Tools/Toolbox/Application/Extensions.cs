using System;
using Toolbox.Tools;

namespace Toolbox.Application
{
    public static class Extensions
    {
        public static RunEnvironment ConvertToEnvironment(this string subject)
        {
            Enum.TryParse(subject, true, out RunEnvironment enviornment)
                .VerifyAssert(x => x == true, $"Invalid environment {subject}");

            return enviornment;
        }
    }
}
