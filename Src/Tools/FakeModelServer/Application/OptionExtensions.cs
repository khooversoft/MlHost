using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace FakeModelServer.Application
{
    public static class OptionExtensions
    {
        public static void Verify(this IOption option)
        {
            option.VerifyNotNull(nameof(option));
            option.Port.VerifyAssert(x => x > 0, "Port number must be greater then 0");
        }
    }
}
