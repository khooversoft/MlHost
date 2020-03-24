using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostApi.Option
{
    public class BlobStoreOption
    {       
        public string? ContainerName { get; set; }

        public string? AccountName { get; set; }

        public string? AccountKey { get; set; }
    }
}
