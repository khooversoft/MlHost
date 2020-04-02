using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Services
{
    public interface IJson
    {
        T Deserialize<T>(string subject);

        string Serialize<T>(T subject);
    }
}
