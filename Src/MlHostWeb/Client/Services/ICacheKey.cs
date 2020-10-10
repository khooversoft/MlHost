using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Services
{
    public interface ICacheKey
    {
        string GetId();
    }
}
