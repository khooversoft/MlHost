using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Services.AzureBlob
{
    internal interface IBlobRepository
    {
        Task Download(string path, Stream toStream);
    }
}
