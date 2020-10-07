using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Toolbox.Services
{
    public interface IPropertyResolver
    {
        [return: NotNullIfNotNull("subject")]
        string? Resolve(string? subject);
    }
}
