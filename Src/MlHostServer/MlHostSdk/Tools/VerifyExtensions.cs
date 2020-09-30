using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Toolbox.Tools;

namespace MlHostSdk.Tools
{
    public static class VerifyExtensions
    {
        private const string _validPattern = "^[a-z][a-z0-9-]*[a-z0-9]+$";

        private static readonly Regex _pattern = new Regex(_validPattern, RegexOptions.Compiled);

        public static string ValidPattern => _validPattern;

        public static string VerifyStoreVector(this string subject, string name) => subject
            .VerifyNotEmpty(name)
            .VerifyAssert(x => _pattern.Match(x).Success, $"{name} is not valid, value={subject}, pattern={_validPattern}");
    }
}
